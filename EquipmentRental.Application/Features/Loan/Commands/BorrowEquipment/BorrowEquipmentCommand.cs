using EquipmentRental.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Loan.Commands.BorrowEquipment
{
    public sealed record BorrowEquipmentCommand(int EquipmentId, DateTimeOffset DueDate) :IRequest<Result>
    {
    }
}
