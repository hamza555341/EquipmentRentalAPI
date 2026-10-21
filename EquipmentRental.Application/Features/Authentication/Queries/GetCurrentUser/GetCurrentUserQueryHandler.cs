using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Authentication.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EquipmentRental.Application.Features.Authentication.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserQueryHandler(
        UserManager<IdentityUser> userManager,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Result<CurrentUserResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<CurrentUserResponse>.Failure(
                Error.Unauthorized(
                    "Auth.Unauthenticated",
                    "Authentication is required."));
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result<CurrentUserResponse>.Failure(
                Error.Unauthorized(
                    "Auth.UserNotFound",
                    "The authenticated user no longer exists."));
        }

        var roles = await _userManager.GetRolesAsync(user);

        var response = new CurrentUserResponse(
            user.Id,
            user.Email,
            user.UserName,
            roles.ToArray());

        return Result<CurrentUserResponse>.Ok(response);
    }
}