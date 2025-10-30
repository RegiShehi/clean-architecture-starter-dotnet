namespace Destination.Domain.Features.Apartments;

using SharedKernel;

public static class ApartmentErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Apartment.NotFound",
        "The apartment with the specified identifier was not found");
}
