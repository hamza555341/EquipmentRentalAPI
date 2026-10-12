using EquipmentRental.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Commands.CreateEquipment
{
    public sealed record CreateEquipmentCommand(string Name,string? Description)
        :IRequest<Result>
    {
    }
}
