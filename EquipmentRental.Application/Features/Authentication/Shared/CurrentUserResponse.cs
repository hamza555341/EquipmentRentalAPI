namespace EquipmentRental.Application.Features.Authentication.Shared;

public sealed record CurrentUserResponse(
    string Id,
    string? Email,
    string? UserName,
    IReadOnlyList<string> Roles);