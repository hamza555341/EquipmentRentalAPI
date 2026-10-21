using EquipmentRental.Api.Controllers.Common;
using EquipmentRental.Application.Features.Authentication.Commands.Login;
using EquipmentRental.Application.Features.Authentication.Commands.Register;
using EquipmentRental.Application.Features.Authentication.Queries.GetCurrentUser;
using EquipmentRental.Application.Features.Authentication.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRental.Api.Controllers;

public sealed class AuthController : ApiBaseController
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("current-user")]
    public async Task<ActionResult<CurrentUserResponse>> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCurrentUserQuery(),
            cancellationToken);

        return HandleResult(result);
    }
}