namespace CleanArchitecture.Domain.Features.Apartments;

using Common;
using SharedKernel;
using ValueObjects;

public sealed class Apartment : Entity
{
    private readonly List<Amenity> _amenities;

    private Apartment()
    {
    }

    public Apartment(
        Guid id,
        Name name,
        Description description,
        Address address,
        Money price,
        Money cleaningFee,
        IEnumerable<Amenity> amenities) : base(id)
    {
        Name = name;
        Description = description;
        Address = address;
        Price = price;
        CleaningFee = cleaningFee;
        _amenities = [..amenities];
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Address Address { get; private set; }
    public Money Price { get; private set; }
    public Money CleaningFee { get; private set; }
    public DateTime? LastBookedOnUtc { get; internal set; }
    public IReadOnlyList<Amenity> Amenities => _amenities;
}
