namespace CleanArchitecture.Application.Features.Bookings.GetBooking;

using Abstractions.Messaging;

public sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;
