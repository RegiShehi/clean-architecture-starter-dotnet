namespace CleanArchitecture.Domain.Common;

public record Money(decimal Amount, Currency Currency)
{
    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies must be equal");
        }

        return first with { Amount = first.Amount + second.Amount };
    }

    public static Money Add(Money first, Money second) => first + second;

    public static Money Zero() => new(0, Currency.None);

    public static Money Zero(Currency currency) => new(0, currency);

    public bool IsZero() => this == Zero(Currency);
}
