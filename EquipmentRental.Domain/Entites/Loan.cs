using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace EquipmentRental.Domain.Entites
{
    public class Loan
    {
        public int Id { get; set; }
        public string UserId { get; set; } =default!;
        public int EquipmentId { get; set; }
        public DateTimeOffset BorrowedAt { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public DateTimeOffset? ReturnedAt { get; set; }





    }
}
