using UtilitiesService.Application.DTO.Utilities;
using UtilitiesService.Application.Interfaces;
using UtilitiesService.Domain.Entities;
using UtilitiesService.Domain.Repositories;

namespace UtilitiesService.Application.Services;

public sealed class UtilitiesService(IUtilitiesRepository utilitiesRepository) : IUtilitiesService
{
    private readonly IUtilitiesRepository _utilitiesRepository = utilitiesRepository;
    public async Task AddUtilityAsync(AddUtilityRequest addUtilityRequest, CancellationToken cancellationToken)
    {
        var utility = new Utility(userId: addUtilityRequest.UserId, name: addUtilityRequest.Name, description: addUtilityRequest.Description, costPerHour: addUtilityRequest.CostPerHour);
        await _utilitiesRepository.AddAsync(utility, cancellationToken);
    }

    public Task DeleteUtilityAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<UtilityListItemDto>> GetUtilitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<UtilityDto> GetUtilityAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUtilityAsync(Guid id, UpdateUtilityRequest updateUtilityRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
