namespace Destination.Application.Features.Apartments.SearchApartments;

using Abstractions.Messaging;
using SharedKernel.Query;

public sealed record SearchApartmentsQuery(QueryOptions Options) : IQuery<PagedResult<SearchApartmentResponse>>;
