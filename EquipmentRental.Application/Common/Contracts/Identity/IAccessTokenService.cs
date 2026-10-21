using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace EquipmentRental.Application.Common.Contracts.Identity
{
    public interface IAccessTokenService
    {
        Task<AccessTokenResult> GenerateTokenAsync(IdentityUser user);
    }
}
