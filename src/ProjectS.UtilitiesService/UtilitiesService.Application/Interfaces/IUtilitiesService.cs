using UtilitiesService.Application.DTO.Utilities;

namespace UtilitiesService.Application.Interfaces;

public interface IUtilitiesService
{
    Task AddUtilityAsync(AddUtilityRequest addUtilityRequest, CancellationToken cancellationToken);

    Task DeleteUtilityAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateUtilityAsync(Guid id, UpdateUtilityRequest updateUtilityRequest, CancellationToken cancellationToken);

    Task<ICollection<UtilityListItemDto>> GetUtilitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<UtilityDto> GetUtilityAsync(Guid id, CancellationToken cancellationToken);
}
