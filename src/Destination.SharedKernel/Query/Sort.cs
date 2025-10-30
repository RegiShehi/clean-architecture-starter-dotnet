namespace Destination.SharedKernel.Query;

public sealed record Sort(string Field, SortDirection Direction = SortDirection.Asc);
