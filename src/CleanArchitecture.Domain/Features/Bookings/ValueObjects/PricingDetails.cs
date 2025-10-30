namespace CleanArchitecture.Domain.Features.Bookings.ValueObjects;

using Common;

public record PricingDetails(Money PriceForPeriod, Money CleaningFee, Money AmenitiesUpCharge, Money TotalPrice);
