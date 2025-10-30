namespace CleanArchitecture.SharedKernel.Query;

public sealed record QueryOptions(
    IReadOnlyList<Filter>? Filters = null,
    IReadOnlyList<Sort>? Sorts = null,
    Page? Page = null
);
