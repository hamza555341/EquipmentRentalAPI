using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Loan.Commands.ReturnLoan
{


    public class ReturnLoanCommandHandler : IRequestHandler<ReturnLoanCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public ReturnLoanCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(ReturnLoanCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null)
                return Result.Failure(Error.Unauthorized());

            var loan = await _unitOfWork.Repository<Domain.Entites.Loan>()
                .GetByIdAsync(request.LoanId, ct);

            if (loan is null)
                return Result.Failure(Error.NotFound("Loan.NotFound", "Loan was not found."));

            if (loan.UserId != userId && !_currentUser.IsInRole("Admin"))
                return Result.Failure(Error.Forbidden("Loan.NotOwner", "You can only return your own loans."));

            if (loan.ReturnedAt is not null)
                return Result.Failure(Error.Conflict("Loan.AlreadyReturned", "Loan has already been returned."));

            loan.ReturnedAt = DateTimeOffset.UtcNow;   // الـ Entity متتبَّعة فمفيش Update
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Ok();
        }
    }
}