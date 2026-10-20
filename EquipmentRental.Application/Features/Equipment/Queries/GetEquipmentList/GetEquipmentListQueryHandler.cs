using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentList.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentList
{
    public class GetEquipmentListQueryHandler : IRequestHandler<GetEquipmentListQuery, Result<IReadOnlyList<EquipmentListItemResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEquipmentListQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<EquipmentListItemResponse>>> Handle(GetEquipmentListQuery request, CancellationToken cancellationToken)
        {
           var EquipRepo= _unitOfWork.Repository<EquipmentRental.Domain.Entites.Equipment>();
           var LoanRepo = _unitOfWork.Repository<EquipmentRental.Domain.Entites.Loan>();




            // get all equipment from the repository
            var equipmentList = await EquipRepo
                           .GetAllAsync(cancellationToken: cancellationToken);

            // get all active loans from the repository
            var activeLoans = await LoanRepo
                          .GetAllAsync(l => l.ReturnedAt == null, cancellationToken);
                

            var borrowedEquipmentIds = activeLoans.Select(l => l.EquipmentId).ToHashSet();

            var equipmentListResponse = equipmentList.Select(e => new EquipmentListItemResponse
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Status = e.Status,
                IsAvaliable = !borrowedEquipmentIds.Contains(e.Id) 
                && e.Status == EquipmentRental.Domain.Entites.EquipmentStatus.active
            }).ToList();


            return Result<IReadOnlyList<EquipmentListItemResponse>>.Ok(equipmentListResponse);

        }
    }
}
