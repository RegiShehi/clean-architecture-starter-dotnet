namespace CleanArchitecture.Api.Controllers.Apartments;

using Application.Abstractions.Messaging;
using Application.Features.Apartments.GetAllApartments;
using Application.Features.Apartments.SearchApartments;
using Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Query;

[ApiController]
[Route("api/apartments")]
public class ApartmentsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllApartments(
        IQueryHandler<GetAllApartmentsQuery, List<ApartmentResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllApartmentsQuery();

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }

    [HttpPost("search")]
    // [ShouldHavePermission(DestinationAction.View, DestinationFeature.Apartments)]
    public async Task<IActionResult> Search([FromBody] QueryOptions options,
        IQueryHandler<SearchApartmentsQuery, PagedResult<SearchApartmentResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new SearchApartmentsQuery(options);

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }
}
