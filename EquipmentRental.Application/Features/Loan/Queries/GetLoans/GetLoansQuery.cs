using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Features.Loan.Queries.GetOverdueLoans.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Loan.Queries.GetLoans
{
    public sealed record GetLoansQuery : IRequest<Result<IReadOnlyList<LoanResponse>>>;

}
