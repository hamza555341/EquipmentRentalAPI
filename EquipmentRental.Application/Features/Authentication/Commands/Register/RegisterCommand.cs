using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Features.Authentication.Shared;
using MediatR;

namespace EquipmentRental.Application.Features.Authentication.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string UserName,
    string Password
) : IRequest<Result<AuthResponse>>;