namespace Destination.Application;

using Abstractions.Messaging;
using Common.Behaviours;
using Domain.Features.Bookings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<PricingService>();

        services.AddHandlersAndDecorators();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }

    private static IServiceCollection AddHandlersAndDecorators(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(IDomainEventListener<>)), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        // Validation decorators
        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        // services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        // Logging decorators
        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        // services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        return services;
    }
}
