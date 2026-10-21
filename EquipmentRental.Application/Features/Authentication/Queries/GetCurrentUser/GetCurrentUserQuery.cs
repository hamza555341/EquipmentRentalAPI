using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Features.Authentication.Shared;
using MediatR;

namespace EquipmentRental.Application.Features.Authentication.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery
    : IRequest<Result<CurrentUserResponse>>;