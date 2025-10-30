namespace Destination.Infrastructure.Repositories;

using Database;
using SharedKernel;

internal abstract class Repository<T> where T : Entity
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext) => DbContext = dbContext;
}
