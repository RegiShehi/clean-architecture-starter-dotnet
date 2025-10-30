namespace Destination.Domain.Features.Bookings.Events;

using SharedKernel;

public sealed record BookingReservedDomainEvent(Guid BookingId) : IDomainEvent;
