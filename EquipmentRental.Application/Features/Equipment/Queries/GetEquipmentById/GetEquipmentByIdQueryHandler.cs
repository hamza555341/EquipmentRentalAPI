using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentById.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentById
{
    public class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, Result<EquipmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEquipmentByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public async Task<Result<EquipmentResponse>> Handle(
            GetEquipmentByIdQuery request,
            CancellationToken cancellationToken)
        {
            var equipRepo = _unitOfWork.Repository<Domain.Entites.Equipment>();

            var equipment = await equipRepo.GetByIdAsync(
                request.Id,
                cancellationToken);

            // Check whether the equipment exists
            if (equipment is null)
            {
                return Result<EquipmentResponse>.Failure(
                    Error.NotFound(
                        "Equipment.NotFound",
                        "Equipment was not found."));
            }

            // Check whether the equipment has an active loan
            var loanRepo = _unitOfWork.Repository<Domain.Entites.Loan>();

            var isBorrowed = await loanRepo.AnyAsync(
                l => l.EquipmentId == equipment.Id && l.ReturnedAt == null,
                cancellationToken);

            var response = new EquipmentResponse
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Description = equipment.Description,
                Status = equipment.Status,

                IsAvaliable = equipment.Status == Domain.Entites.EquipmentStatus.active
                              && !isBorrowed,

                CreatedAt = equipment.CreatedAt
            };

            return Result<EquipmentResponse>.Ok(response);
        }

    }
}
