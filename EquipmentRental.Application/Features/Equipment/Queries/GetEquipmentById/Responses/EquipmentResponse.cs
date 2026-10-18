using EquipmentRental.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentById.Responses
{
    public class EquipmentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public EquipmentStatus Status { get; set; }
        public bool IsAvaliable { get; set; }
        public DateTimeOffset CreatedAt { get; set; }


    }
}
