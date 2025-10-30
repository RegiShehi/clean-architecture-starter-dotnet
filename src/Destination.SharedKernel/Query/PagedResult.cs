namespace Destination.SharedKernel.Query;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total, int PageNumber, int PageSize)
{
    public PagedResult<TResult> Map<TResult>(Func<T, TResult> selector) =>
        new([.. Items.Select(selector)], Total, PageNumber, PageSize);
}
