using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Loan.Queries.GetOverdueLoans.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Loan.Queries.GetLoans
{
    public class GetLoansQueryHandler : IRequestHandler<GetLoansQuery, Result<IReadOnlyList<LoanResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetLoansQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<IReadOnlyList<LoanResponse>>> Handle(GetLoansQuery request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null)
                return Result<IReadOnlyList<LoanResponse>>.Failure(Error.Unauthorized());

            var loanRepo = _unitOfWork.Repository<Domain.Entites.Loan>();

            // Admin يشوف الكل، Member يشوف بتاعه بس
            var loans = _currentUser.IsInRole("Admin")
                ? await loanRepo.GetAllAsync(cancellationToken: ct)
                : await loanRepo.GetAllAsync(l => l.UserId == userId, ct);

            var equipmentIds = loans.Select(l => l.EquipmentId).Distinct().ToList();
            var equipment = await _unitOfWork.Repository<Domain.Entites.Equipment>()
                .GetAllAsync(e => equipmentIds.Contains(e.Id), ct);
            var names = equipment.ToDictionary(e => e.Id, e => e.Name);

            var now = DateTimeOffset.UtcNow;

            var response = loans
                .OrderByDescending(l => l.BorrowedAt)
                .Select(l => new LoanResponse
                {
                    Id = l.Id,
                    EquipmentId = l.EquipmentId,
                    EquipmentName = names.GetValueOrDefault(l.EquipmentId, "Unknown"),
                    UserId = l.UserId,
                    BorrowedAt = l.BorrowedAt,
                    DueDate = l.DueDate,
                    ReturnedAt = l.ReturnedAt,
                    IsOverdue = l.ReturnedAt == null && l.DueDate < now
                })
                .ToList();

            return Result<IReadOnlyList<LoanResponse>>.Ok(response);
        }
    }
}
