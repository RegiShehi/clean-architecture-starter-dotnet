namespace CleanArchitecture.Domain.Common;

public record Currency
{
    internal static readonly Currency None = new("");
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");

    private Currency(string code) => Code = code;

    private static IReadOnlyCollection<Currency> All => [Usd, Eur];

    public string Code { get; init; }

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ??
               throw new ArgumentException("The currency code is invalid");
    }
}
