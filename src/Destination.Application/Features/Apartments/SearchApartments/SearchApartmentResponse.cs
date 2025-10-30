namespace Destination.Application.Features.Apartments.SearchApartments;

using Domain.Features.Apartments;

public sealed class SearchApartmentResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string City { get; init; } = "";
    public string Country { get; init; } = "";
    public decimal Price { get; init; }
    public decimal CleaningFee { get; init; }
    public string Currency { get; init; } = "";

    public DateTime? LastBookedOnUtc { get; init; }

    // public string[] Amenities { get; init; } = [];

    public static SearchApartmentResponse ToResponse(Apartment r) => new()
    {
        Id = r.Id,
        Name = r.Name.Value,
        Description = r.Description.Value,
        City = r.Address.City,
        Country = r.Address.Country,
        Price = r.Price.Amount,
        CleaningFee = r.CleaningFee.Amount,
        Currency = r.Price.Currency.ToString(),
        LastBookedOnUtc = r.LastBookedOnUtc
        // Amenities = r.Apartment.Amenities.Select(a => a.Code).ToArray(),
    };
}
