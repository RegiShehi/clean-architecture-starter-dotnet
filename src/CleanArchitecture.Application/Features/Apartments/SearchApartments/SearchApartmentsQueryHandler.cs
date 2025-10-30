namespace CleanArchitecture.Application.Features.Apartments.SearchApartments;

using Abstractions.Messaging;
using Domain.Features.Apartments;
using SharedKernel;
using SharedKernel.Query;

internal sealed class
    SearchApartmentsQueryHandler(IApartmentRepository apartmentRepository)
    : IQueryHandler<SearchApartmentsQuery, PagedResult<SearchApartmentResponse>>
{
    public async Task<Result<PagedResult<SearchApartmentResponse>>> Handle(SearchApartmentsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await apartmentRepository.SearchAsync(query.Options, cancellationToken);

        return result.Map(SearchApartmentResponse.ToResponse);
    }
}
