using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentById
{
    public class GetEquipmentByIdQueryValidator :AbstractValidator<GetEquipmentByIdQuery>
    {

        public GetEquipmentByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");


        }

    }
}
