using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Commands.UpdateEquipmentStatus
{
    public sealed class UpdateEquipmentStatusCommandHandler
        : IRequestHandler<UpdateEquipmentStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEquipmentStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            UpdateEquipmentStatusCommand request,
            CancellationToken cancellationToken)
        {
            var equipmentRepo =
                _unitOfWork.Repository<Domain.Entites.Equipment>();

            var equipment = await equipmentRepo.GetByIdAsync(
                request.EquipmentId,
                cancellationToken);

            if (equipment is null)
            {
                return Result.Failure(
                    Error.NotFound(
                        "Equipment.NotFound",
                        "Equipment was not found."));
            }

            equipment.Status = request.Status;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
