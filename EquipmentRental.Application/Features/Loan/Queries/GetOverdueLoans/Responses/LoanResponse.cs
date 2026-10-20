using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Loan.Queries.GetOverdueLoans.Responses
{
    public class LoanResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; } = default!;
        public DateTimeOffset BorrowedAt { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public DateTimeOffset? ReturnedAt { get; set; }
        public bool IsOverdue { get; set; }
    }
}
