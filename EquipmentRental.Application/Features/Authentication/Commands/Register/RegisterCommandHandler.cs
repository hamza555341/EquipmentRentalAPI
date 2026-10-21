using EquipmentRental.Application.Common.Contracts.Identity;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Authentication.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EquipmentRental.Application.Features.Authentication.Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private const string MemberRole = "Member";

    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAccessTokenService _accessTokenService;

    public RegisterCommandHandler(
        UserManager<IdentityUser> userManager,
        IAccessTokenService accessTokenService)
    {
        _userManager = userManager;
        _accessTokenService = accessTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var userName = request.UserName.Trim();

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return Result<AuthResponse>.Failure(
                Error.Conflict(
                    "Auth.EmailAlreadyExists",
                    "An account with this email already exists."));
        }

        var user = new IdentityUser
        {
            Email = email,
            UserName = userName
        };

        var createResult = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                createResult.Errors.Select(e => e.Description));

            return Result<AuthResponse>.Failure(
                Error.Validation(
                    "Auth.RegistrationFailed",
                    errors));
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            MemberRole);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            var errors = string.Join(
                "; ",
                roleResult.Errors.Select(e => e.Description));

            return Result<AuthResponse>.Failure(
                Error.Failure(
                    "Auth.RoleAssignmentFailed",
                    errors));
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