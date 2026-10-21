using EquipmentRental.Application.Common.Contracts.Identity;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Authentication.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EquipmentRental.Application.Features.Authentication.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAccessTokenService _accessTokenService;

    public LoginCommandHandler(
        UserManager<IdentityUser> userManager,
        IAccessTokenService accessTokenService)
    {
        _userManager = userManager;
        _accessTokenService = accessTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(
            request.Email.Trim());

        if (user is null ||
            !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result<AuthResponse>.Failure(
                Error.Unauthorized(
                    "Auth.InvalidCredentials",
                    "Email or password is invalid."));
        }

        var roles = await _userManager.GetRolesAsync(user);

        var tokenResult = await _accessTokenService
            .GenerateTokenAsync(user);

        var response = new AuthResponse(
            user.Id,
            user.Email!,
            user.UserName!,
            roles.ToArray(),
            tokenResult.Token,
            tokenResult.ExpiresAtUtc);

        return Result<AuthResponse>.Ok(response);
    }
}