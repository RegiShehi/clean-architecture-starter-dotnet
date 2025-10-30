namespace CleanArchitecture.SharedKernel.Query;

public sealed record Filter(string Field, FilterOperator Operator, object? Value, object? Extra = null);
