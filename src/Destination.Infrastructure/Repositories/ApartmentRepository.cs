namespace Destination.Infrastructure.Repositories;

using Common.Querying;
using Database;
using Domain.Features.Apartments;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Query;

internal sealed class ApartmentRepository(ApplicationDbContext dbContext)
    : Repository<Apartment>(dbContext), IApartmentRepository
{
    public async Task<Apartment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var apartment = await DbContext.Set<Apartment>().FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        return apartment;
    }

    public async Task<List<Apartment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var apartments = await DbContext.Set<Apartment>().ToListAsync(cancellationToken);

        return apartments;
    }

    public async Task<PagedResult<Apartment>> SearchAsync(QueryOptions options,
        CancellationToken cancellationToken = default)
    {
        var apartments = await DbContext.Set<Apartment>().AsNoTracking()
            // .Select(a => new ApartmentSearchRow(a))
            .FilterBy(options.Filters)
            .SortBy(options.Sorts)
            .ToPagedResultAsync(options.Page, cancellationToken);

        return apartments;
    }
}
