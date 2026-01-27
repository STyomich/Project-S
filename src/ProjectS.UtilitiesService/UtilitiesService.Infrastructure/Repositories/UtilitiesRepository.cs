using UtilitiesService.Domain.Entities;
using UtilitiesService.Domain.Repositories;
using UtilitiesService.Infrastructure.Persistence;
using MongoDB.Driver;

namespace UtilitiesService.Infrastructure.Repositories;

public sealed class UtilitiesRepository(UtilitiesDbContext dbContext) : IUtilitiesRepository
{
    private readonly IMongoCollection<Utility> _utilitiesCollection = dbContext.Database.GetCollection<Utility>("utilities");


}
