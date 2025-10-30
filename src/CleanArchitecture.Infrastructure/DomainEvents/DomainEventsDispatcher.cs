﻿namespace CleanArchitecture.Infrastructure.DomainEvents;

using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

internal sealed class DomainEventsDispatcher(IServiceProvider serviceProvider) : IDomainEventsDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeDictionary = new();
    private static readonly ConcurrentDictionary<Type, Type> WrapperTypeDictionary = new();

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            using var scope = serviceProvider.CreateScope();

            var domainEventType = domainEvent.GetType();
            var handlerType = HandlerTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(IDomainEventListener<>).MakeGenericType(et));

            var handlers = scope.ServiceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                if (handler is null)
                {
                    continue;
                }

                var handlerWrapper = HandlerWrapper.Create(handler, domainEventType);

                await handlerWrapper.Handle(domainEvent, cancellationToken);
            }
        }
    }

    private abstract class HandlerWrapper
    {
        public abstract Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken);

        public static HandlerWrapper Create(object handler, Type domainEventType)
        {
            var wrapperType = WrapperTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(HandlerWrapper<>).MakeGenericType(et));

            var instance = Activator.CreateInstance(wrapperType, handler);
            return instance as HandlerWrapper
                   ?? throw new InvalidOperationException(
                       $"Could not create {wrapperType} for handler {handler.GetType().FullName}.");
        }
    }

    private sealed class HandlerWrapper<T>(object handler) : HandlerWrapper where T : IDomainEvent
    {
        private readonly IDomainEventListener<T> _handler = (IDomainEventListener<T>)handler;

        public override async Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
            await _handler.Handle((T)domainEvent, cancellationToken);
    }
}
