using UtilitiesService.Domain.Entities;

namespace UtilitiesService.Domain.Repositories;

public interface IUtilitiesRepository
{
    Task<List<Utility>> GetAllAsync(CancellationToken cancellationToken);

    Task<Utility?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Utility utility, CancellationToken cancellationToken);

    Task UpdateAsync(Guid id, Utility utility, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
