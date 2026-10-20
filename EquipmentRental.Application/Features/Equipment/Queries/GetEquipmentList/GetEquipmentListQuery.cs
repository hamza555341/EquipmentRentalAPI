using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentList.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentList
{
    public sealed record GetEquipmentListQuery :IRequest<Result<IReadOnlyList<EquipmentListItemResponse>>>
    {
    }
}
