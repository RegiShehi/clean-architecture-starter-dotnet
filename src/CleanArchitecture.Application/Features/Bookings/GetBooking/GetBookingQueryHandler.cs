namespace CleanArchitecture.Application.Features.Bookings.GetBooking;

using Abstractions.Messaging;
using SharedKernel;

internal sealed class GetBookingQueryHandler
    : IQueryHandler<GetBookingQuery, BookingResponse>
{
    public Task<Result<BookingResponse>> Handle(GetBookingQuery query, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
