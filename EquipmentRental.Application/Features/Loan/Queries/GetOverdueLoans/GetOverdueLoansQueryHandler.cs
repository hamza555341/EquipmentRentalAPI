using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Features.Loan.Queries.GetOverdueLoans.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Loan.Queries.GetOverdueLoans
{
    public class GetOverdueLoansQueryHandler
       : IRequestHandler<GetOverdueLoansQuery, Result<IReadOnlyList<LoanResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetOverdueLoansQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<IReadOnlyList<LoanResponse>>> Handle(GetOverdueLoansQuery request, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            var loans = await _unitOfWork.Repository<Domain.Entites.Loan>()
                .GetAllAsync(l => l.ReturnedAt == null && l.DueDate < now, ct);

            var equipmentIds = loans.Select(l => l.EquipmentId).Distinct().ToList();
            var equipment = await _unitOfWork.Repository<Domain.Entites.Equipment>()
                .GetAllAsync(e => equipmentIds.Contains(e.Id), ct);
            var names = equipment.ToDictionary(e => e.Id, e => e.Name);

            var response = loans
                .OrderBy(l => l.DueDate)
                .Select(l => new LoanResponse
                {
                    Id = l.Id,
                    EquipmentId = l.EquipmentId,
                    EquipmentName = names.GetValueOrDefault(l.EquipmentId, "Unknown"),
                    UserId = l.UserId,
                    BorrowedAt = l.BorrowedAt,
                    DueDate = l.DueDate,
                    ReturnedAt = l.ReturnedAt,
                    IsOverdue = true
                })
                .ToList();

            return Result<IReadOnlyList<LoanResponse>>.Ok(response);
        }
    }
}
