using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Loan.Commands.BorrowEquipment
{
    public class BorrowEquipmentCommandValidator: AbstractValidator<BorrowEquipmentCommand>
    {
        public BorrowEquipmentCommandValidator()
        {
            RuleFor(x => x.EquipmentId)
                .GreaterThan(0).WithMessage("EquipmentId must be greater than 0.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTimeOffset.Now).WithMessage("DueDate must be in the future.");

        }





    }
}
