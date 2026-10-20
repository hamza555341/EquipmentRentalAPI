using EquipmentRental.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Loan.Commands.ReturnLoan
{
    public sealed record ReturnLoanCommand(int LoanId) : IRequest<Result>;

}
