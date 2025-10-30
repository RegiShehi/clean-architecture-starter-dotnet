namespace Destination.SharedKernel;

public sealed record ValidationError : Error
{
    public ValidationError(IEnumerable<Error> errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        var arr = errors as Error[] ?? [.. errors];
        Errors = Array.AsReadOnly(arr);
    }

    public IReadOnlyList<Error> Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure)
            .Select(r => r.Error));
}
