using EquipmentRental.Application.Common.Results;
using EquipmentRental.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Commands.UpdateEquipmentStatus
{
    public sealed record UpdateEquipmentStatusCommand(
     int EquipmentId,
     EquipmentStatus Status
 ) : IRequest<Result>;
}
