using EquipmentRental.Api.Controllers.Common;
using EquipmentRental.Application.Features.Loan.Commands.BorrowEquipment;
using EquipmentRental.Application.Features.Loan.Commands.ReturnLoan;
using EquipmentRental.Application.Features.Loan.Queries.GetLoans;
using EquipmentRental.Application.Features.Loan.Queries.GetOverdueLoans;
using EquipmentRental.Application.Features.Loan.Queries.GetOverdueLoans.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRental.Api.Controllers
{
    public class LoanController:ApiBaseController
    {
        private readonly ISender _sender;

        public LoanController(ISender sender)
        {
            _sender = sender;
        }


        // POST api/loans
        [HttpPost]
        public async Task<IActionResult> Borrow([FromBody] BorrowEquipmentCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess ? StatusCode(StatusCodes.Status201Created) : HandleResult(result);
        }

        // GET api/loans  (Member: بتاعته | Admin: الكل)
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<LoanResponse>>> GetLoans(CancellationToken ct)
            => HandleResult(await _sender.Send(new GetLoansQuery(), ct));

        // GET api/loans/overdue  (Admin فقط)
        [HttpGet("overdue")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<LoanResponse>>> GetOverdue(CancellationToken ct)
            => HandleResult(await _sender.Send(new GetOverdueLoansQuery(), ct));

        // POST api/loans/5/return
        [HttpPost("{id:int}/return")]
        public async Task<IActionResult> Return(int id, CancellationToken ct)
            => HandleResult(await _sender.Send(new ReturnLoanCommand(id), ct));
    }
}

