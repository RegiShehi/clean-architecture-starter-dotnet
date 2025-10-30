namespace Destination.Application.Features.Apartments.GetAllApartments;

using Abstractions.Messaging;
using Domain.Features.Apartments;
using SharedKernel;

internal sealed class
    GetAllApartmentsQueryHandler(IApartmentRepository apartmentRepository)
    : IQueryHandler<GetAllApartmentsQuery, List<ApartmentResponse>>
{
    public async Task<Result<List<ApartmentResponse>>> Handle(
        GetAllApartmentsQuery request,
        CancellationToken cancellationToken)
    {
        var apartments = (await apartmentRepository.GetAllAsync(cancellationToken))
            .Select(apartment => new ApartmentResponse
            {
                Id = apartment.Id,
                Name = apartment.Name.Value,
                Description = apartment.Description.Value,
                Price = apartment.Price.Amount,
                Currency = apartment.Price.Currency.Code,
                Address = new AddressResponse
                {
                    Street = apartment.Address.Street,
                    City = apartment.Address.City,
                    Country = apartment.Address.Country,
                    ZipCode = apartment.Address.ZipCode
                }
            })
            .ToList();

        return apartments;
    }
}
