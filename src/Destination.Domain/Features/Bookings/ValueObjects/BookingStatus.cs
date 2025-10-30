namespace Destination.Domain.Features.Bookings.ValueObjects;

public enum BookingStatus
{
    None = 0,
    ReservationMade = 1,
    Confirmed = 2,
    Rejected = 3,
    Cancelled = 4,
    Completed = 5
}
