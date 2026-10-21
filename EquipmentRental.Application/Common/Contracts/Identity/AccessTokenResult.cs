using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Common.Contracts.Identity
{
    public sealed record AccessTokenResult(
      string Token,
      DateTime ExpiresAtUtc);
}
