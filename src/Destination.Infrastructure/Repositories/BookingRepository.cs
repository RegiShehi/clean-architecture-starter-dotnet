namespace Destination.Infrastructure.Repositories;

using Database;
using Domain.Common;
using Domain.Features.Apartments;
using Domain.Features.Bookings;
using Domain.Features.Bookings.ValueObjects;
using Microsoft.EntityFrameworkCore;

internal sealed class BookingRepository : Repository<Booking>, IBookingRepository
{
    private static readonly BookingStatus[] ActiveBookingStatuses =
    [
        BookingStatus.ReservationMade,
        BookingStatus.Confirmed,
        BookingStatus.Completed
    ];

    public BookingRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public async Task<bool> IsOverlappingAsync(
        Apartment apartment,
        DateRange duration,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Booking>()
            .AnyAsync(
                b =>
                    b.ApartmentId == apartment.Id &&
                    b.Duration.Start <= duration.End &&
                    b.Duration.End >= duration.Start &&
                    ActiveBookingStatuses.Contains(b.Status),
                cancellationToken);
    }

    public void Add(Booking booking) => DbContext.Add(booking);
}
