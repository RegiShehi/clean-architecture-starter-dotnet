namespace CleanArchitecture.Infrastructure.Common.Querying;

using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

internal static class PropertyAccessorCache
{
    private static readonly ConcurrentDictionary<(Type, string), LambdaExpression> Cache = new();

    public static Expression GetMemberAccess(Expression root, string path)
    {
        var current = root;

        foreach (var part in path.Split('.'))
        {
            var prop = current.Type.GetProperty(part,
                           BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                       ?? throw new InvalidOperationException($"Unknown field '{path}' on {current.Type.Name}.");

            current = Expression.Property(current, prop);
        }

        return current;
    }

    public static Expression<Func<T, object?>> GetAccessor<T>(string path)
    {
        var key = (typeof(T), path);

        if (Cache.TryGetValue(key, out var cached))
        {
            return (Expression<Func<T, object?>>)cached;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var body = GetMemberAccess(parameter, path);
        var lambda = Expression.Lambda<Func<T, object?>>(Expression.Convert(body, typeof(object)), parameter);

        Cache[key] = lambda;

        return lambda;
    }
}
