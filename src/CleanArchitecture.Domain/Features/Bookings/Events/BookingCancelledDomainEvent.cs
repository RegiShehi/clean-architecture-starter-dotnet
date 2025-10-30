namespace CleanArchitecture.Domain.Features.Bookings.Events;

using SharedKernel;

public sealed record BookingCancelledDomainEvent(Guid Id) : IDomainEvent;
