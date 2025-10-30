namespace CleanArchitecture.Domain.Features.Bookings.Events;

using SharedKernel;

public sealed record BookingRejectedDomainEvent(Guid Id) : IDomainEvent;
