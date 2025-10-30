namespace CleanArchitecture.Application.Features.Bookings.ReserveBooking;

using Abstractions.Messaging;
using Abstractions.Persistence;
using Abstractions.Time;
using Common.Exceptions;
using Domain.Common;
using Domain.Features.Apartments;
using Domain.Features.Bookings;
using SharedKernel;

internal sealed class ReserveBookingCommandHandler(
    IApartmentRepository apartmentRepository,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    PricingService pricingService,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<ReserveBookingCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ReserveBookingCommand command, CancellationToken cancellationToken)
    {
        var apartment = await apartmentRepository.GetByIdAsync(command.ApartmentId, cancellationToken);

        if (apartment is null)
        {
            return Result.Failure<Guid>(ApartmentErrors.NotFound);
        }

        var duration = DateRange.Create(command.StartDate, command.EndDate);

        if (await bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }

        try
        {
            var booking = Booking.Reserve(apartment, Guid.NewGuid(), duration, dateTimeProvider.UtcNow, pricingService);

            bookingRepository.Add(booking);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return booking.Id;
        }
        catch (ConcurrencyException)
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }
    }
}
