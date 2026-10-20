using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Commands.UpdateEquipmentStatus
{
    public sealed class UpdateEquipmentStatusCommandValidator
      : AbstractValidator<UpdateEquipmentStatusCommand>
    {
        public UpdateEquipmentStatusCommandValidator()
        {
            RuleFor(x => x.EquipmentId)
                .GreaterThan(0)
                .WithMessage("EquipmentId must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid equipment status.");
        }
    }
}
