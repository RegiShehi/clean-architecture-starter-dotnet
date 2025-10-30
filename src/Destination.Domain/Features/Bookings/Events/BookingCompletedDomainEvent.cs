namespace Destination.Domain.Features.Bookings.Events;

using SharedKernel;

public sealed record BookingCompletedDomainEvent(Guid Id) : IDomainEvent;
