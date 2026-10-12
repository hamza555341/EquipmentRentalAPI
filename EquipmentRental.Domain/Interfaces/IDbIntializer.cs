using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Domain.Interfaces
{
    public interface IDbIntializer
    {
        Task InitializeDataAsync();

    }
}
