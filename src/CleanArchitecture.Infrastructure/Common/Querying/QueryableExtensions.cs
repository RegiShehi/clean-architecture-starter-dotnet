namespace CleanArchitecture.Infrastructure.Common.Querying;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Query;

public static class QueryableExtensions
{
    public static IQueryable<T> FilterBy<T>(this IQueryable<T> query, IReadOnlyList<Filter>? filters)
    {
        if (filters is null || filters.Count == 0)
        {
            return query;
        }

        Expression<Func<T, bool>>? combined = null;

        foreach (var filter in filters)
        {
            var predicate = FilterExpressionBuilder.BuildPredicate<T>(filter);

            combined = combined is null ? predicate : combined.And(predicate);
        }

        return combined is null ? query : query.Where(combined);
    }

    public static IQueryable<T> SortBy<T>(this IQueryable<T> query, IReadOnlyList<Sort>? sorts)
    {
        if (sorts is null || sorts.Count == 0)
        {
            return query;
        }

        IOrderedQueryable<T>? ordered = null;

        foreach (var sort in sorts)
        {
            var key = PropertyAccessorCache.GetAccessor<T>(sort.Field);

            ordered = (ordered, sort.Direction) switch
            {
                (null, SortDirection.Asc) => query.OrderBy(key),
                (null, _) => query.OrderByDescending(key),
                (_, SortDirection.Asc) => ordered.ThenBy(key),
                _ => ordered.ThenByDescending(key)
            };
        }

        return ordered ?? query;
    }

    public static Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> source,
        Page? page,
        CancellationToken cancellationToken = default)
        => source.ToPagedResultAsync(page, x => x, true, cancellationToken);

    private static async Task<PagedResult<TResult>> ToPagedResultAsync<T, TResult>(
        this IQueryable<T> source,
        Page? page,
        Expression<Func<T, TResult>> selector,
        bool includeTotal = true,
        CancellationToken cancellationToken = default)
    {
        page ??= new Page();

        var total = includeTotal ? await source.CountAsync(cancellationToken) : 0;

        // Apply pagination and project
        var items = await source
            .Skip(page.Skip)
            .Take(page.Take)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return new PagedResult<TResult>(items, total, page.Number, page.Size);
    }

    private static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = Expression.AndAlso(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
