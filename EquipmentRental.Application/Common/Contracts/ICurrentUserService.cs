using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Common.Contracts
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
    }
}
