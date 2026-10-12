using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Commands.CreateEquipment
{
    public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateEquipmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        public async Task<Result> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
        {

          var equipment = new Domain.Entites.Equipment
          {
              Name = request.Name,
              Description = request.Description,
              Status= EquipmentStatus.active,
              CreatedAt = DateTimeOffset.UtcNow

          };


            var repository = _unitOfWork.Repository<Domain.Entites.Equipment>();

            await repository.AddAsync(equipment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();

        }
    }
}
