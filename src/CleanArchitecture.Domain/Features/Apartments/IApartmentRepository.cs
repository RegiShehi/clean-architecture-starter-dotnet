namespace CleanArchitecture.Domain.Features.Apartments;

using SharedKernel.Query;

public interface IApartmentRepository
{
    Task<Apartment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Apartment>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<Apartment>> SearchAsync(QueryOptions options,
        CancellationToken cancellationToken = default);
}
