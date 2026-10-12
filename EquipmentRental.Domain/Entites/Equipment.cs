using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Domain.Entites
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;    
        public string? Description { get; set; }
        public EquipmentStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }



    }
}
