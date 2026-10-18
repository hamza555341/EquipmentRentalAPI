using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
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


        public async Task<Result<EquipmentResponse>> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
        {
           var EquipRepo =  _unitOfWork.Repository<Domain.Entites.Equipment>();

            var equipment = await EquipRepo.GetByIdAsync(request.Id,cancellationToken);


             var response = new EquipmentResponse
             {
                 Id = equipment!.Id,
                 Name = equipment.Name,
                 Description = equipment.Description,
                 Status = equipment.Status,
                 IsAvaliable = equipment.Status == Domain.Entites.EquipmentStatus.active,
                 CreatedAt = equipment.CreatedAt
             };



            return Result<EquipmentResponse>.Ok(response);

        }
    }
}
