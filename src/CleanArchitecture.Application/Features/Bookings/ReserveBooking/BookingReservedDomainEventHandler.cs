namespace CleanArchitecture.Application.Features.Bookings.ReserveBooking;

using Domain.Features.Bookings;
using Domain.Features.Bookings.Events;
using Microsoft.Extensions.Logging;
using SharedKernel;

internal sealed class BookingReservedDomainEventHandler(
    IBookingRepository bookingRepository,
    ILogger<BookingReservedDomainEventHandler> logger)
    : IDomainEventListener<BookingReservedDomainEvent>
{
    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

        logger.LogInformation("Reserving booking with id: {BookingId}", booking?.Id);
    }
}
