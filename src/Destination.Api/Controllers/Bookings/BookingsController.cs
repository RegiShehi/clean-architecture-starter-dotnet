namespace Destination.Api.Controllers.Bookings;

using Application.Abstractions.Messaging;
using Application.Features.Bookings.GetBooking;
using Application.Features.Bookings.ReserveBooking;
using Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    [HttpGet("{id:guid}")]
    [EndpointName("GetBookingById")]
    public async Task<IActionResult> GetBooking(Guid id,
        IQueryHandler<GetBookingQuery, BookingResponse> handler, CancellationToken cancellationToken)
    {
        var query = new GetBookingQuery(id);

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(value => Ok(value), CustomResults.Problem);
    }

    [HttpPost]
    public async Task<IActionResult> ReserveBooking(
        ReserveBookingRequest request,
        ICommandHandler<ReserveBookingCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new ReserveBookingCommand(
            request.ApartmentId,
            request.UserId,
            request.StartDate,
            request.EndDate);

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(
            id => CreatedAtRoute("GetBookingById", new { id }, new { id }),
            CustomResults.Problem
        );
    }
}
