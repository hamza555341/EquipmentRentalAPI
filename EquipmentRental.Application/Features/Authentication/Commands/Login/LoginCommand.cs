using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Features.Authentication.Shared;
using MediatR;

namespace EquipmentRental.Application.Features.Authentication.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;