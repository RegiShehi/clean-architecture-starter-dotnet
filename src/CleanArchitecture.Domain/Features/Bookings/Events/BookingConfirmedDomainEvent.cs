namespace CleanArchitecture.Domain.Features.Bookings.Events;

using SharedKernel;

public sealed record BookingConfirmedDomainEvent(Guid Id) : IDomainEvent;
