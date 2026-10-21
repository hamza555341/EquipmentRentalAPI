namespace EquipmentRental.Application.Features.Authentication.Shared;

public sealed record AuthResponse(
    string UserId,
    string Email,
    string UserName,
    IReadOnlyList<string> Roles,
    string AccessToken,
    DateTime ExpiresAtUtc);