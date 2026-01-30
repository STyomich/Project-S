using UtilitiesService.Domain.Entities;
using UtilitiesService.Domain.Repositories;
using UtilitiesService.Infrastructure.Persistence;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace UtilitiesService.Infrastructure.Repositories;

public sealed class UtilitiesRepository(UtilitiesDbContext dbContext) : IUtilitiesRepository
{
    private readonly IMongoCollection<Utility> _utilitiesCollection = dbContext.Database.GetCollection<Utility>("utilities");

    public IQueryable<Utility> Query()
    {
        return _utilitiesCollection.AsQueryable();
    }

    public async Task AddAsync(Utility utility, CancellationToken cancellationToken)
    {
        await _utilitiesCollection.InsertOneAsync(utility, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _utilitiesCollection.DeleteOneAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<List<Utility>> GetAllAsync(CancellationToken cancellationToken) => await Query().ToListAsync(cancellationToken);

    public async Task<Utility?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Query().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Guid id, Utility utility, CancellationToken cancellationToken)
    {
        await _utilitiesCollection.ReplaceOneAsync(u => u.Id == id, utility, cancellationToken: cancellationToken);
    }
}
