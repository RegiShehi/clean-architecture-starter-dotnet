namespace CleanArchitecture.Api.Infrastructure;

using System.Collections.Concurrent;
using System.Reflection;
using Application.Abstractions.Messaging;
using SharedKernel;

public sealed class Sender(IServiceProvider sp) : ISender
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> HandleCache = new();

    public async Task<Result> Send(ICommand command, CancellationToken cancellationToken = default)
    {
        var handlerInterface = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = sp.GetRequiredService(handlerInterface);

        var handle = GetHandleMethod(handlerInterface);
        var task = (Task<Result>)handle.Invoke(handler, [command, cancellationToken])!;

        return await task.ConfigureAwait(false);
    }

    public async Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
    {
        var handlerInterface = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var handler = sp.GetRequiredService(handlerInterface);

        var handle = GetHandleMethod(handlerInterface);
        var task = (Task<Result<TResponse>>)handle.Invoke(handler, [command, cancellationToken])!;

        return await task.ConfigureAwait(false);
    }

    public async Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
    {
        var handlerInterface = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = sp.GetRequiredService(handlerInterface);

        var handle = GetHandleMethod(handlerInterface);
        var task = (Task<Result<TResponse>>)handle.Invoke(handler, [query, cancellationToken])!;

        return await task.ConfigureAwait(false);
    }

    private static MethodInfo GetHandleMethod(Type handlerInterface)
        => HandleCache.GetOrAdd(handlerInterface, static t =>
        {
            // Get the interface's Handle method (works even if the implementation is explicit)
            var mi = t.GetMethod("Handle", BindingFlags.Public | BindingFlags.Instance);

            if (mi is null)
            {
                throw new InvalidOperationException($"Could not find Handle on {t.FullName}.");
            }

            return mi;
        });
}
