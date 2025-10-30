namespace CleanArchitecture.Application.Features.Bookings.ReserveBooking;

using Abstractions.Messaging;

public record ReserveBookingCommand(Guid ApartmentId, Guid UserId, DateOnly StartDate, DateOnly EndDate)
    : ICommand<Guid>;
