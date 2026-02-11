namespace UtilitiesService.Application.DTO.Utilities;

public sealed record UpdateUtilityRequest(string Name, string Description, decimal CostPerHour);
