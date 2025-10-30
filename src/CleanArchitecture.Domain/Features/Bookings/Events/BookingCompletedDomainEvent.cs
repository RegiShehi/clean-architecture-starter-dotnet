namespace CleanArchitecture.Domain.Features.Bookings.Events;

using SharedKernel;

public sealed record BookingCompletedDomainEvent(Guid Id) : IDomainEvent;
