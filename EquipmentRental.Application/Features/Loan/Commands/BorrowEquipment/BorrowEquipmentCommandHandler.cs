using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using MediatR;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Loan.Commands.BorrowEquipment
{
    public class BorrowEquipmentCommandHandler : IRequestHandler<BorrowEquipmentCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public BorrowEquipmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(BorrowEquipmentCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null)
                return Result.Failure(Error.Unauthorized());

            var equipment = await _unitOfWork.Repository<Domain.Entites.Equipment>()
                .GetByIdAsync(request.EquipmentId, ct);

            if (equipment is null)
                return Result.Failure(Error.NotFound("Equipment.NotFound", "Equipment was not found."));

            if (equipment.Status != Domain.Entites.EquipmentStatus.active)
                return Result.Failure(Error.Conflict("Equipment.UnderMaintenance", "Equipment is under maintenance."));

            var now = DateTimeOffset.UtcNow;
            if (request.DueDate <= DateTimeOffset.Now)
                return Result.Failure(Error.Validation("Loan.InvalidDueDate", "Due date must be in the future."));

            var loanRepo = _unitOfWork.Repository<Domain.Entites.Loan>();

            var alreadyBorrowed = await loanRepo.AnyAsync(
                l => l.EquipmentId == equipment.Id && l.ReturnedAt == null, ct);

            if (alreadyBorrowed)
                return Result.Failure(Error.Conflict("Equipment.AlreadyBorrowed", "Equipment is already borrowed."));

            await loanRepo.AddAsync(new Domain.Entites.Loan
            {
                EquipmentId = equipment.Id,
                UserId = userId,
                BorrowedAt = now,
                DueDate = request.DueDate
            }, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}

