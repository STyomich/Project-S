namespace UtilitiesService.Application.DTO.Utilities;

public sealed record AddUtilityRequest(Guid UserId, string Name, string Description, decimal CostPerHour);
