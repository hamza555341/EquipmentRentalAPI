using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentById.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentById
{
    public sealed record GetEquipmentByIdQuery(int Id) :IRequest<Result<EquipmentResponse>>
    {

    }
}
