using EquipmentRental.Application.Common.Contracts;
using System.Security.Claims;

namespace EquipmentRental.Api.Helper  
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public CurrentUserService(IHttpContextAccessor contextAccessor) => _contextAccessor = contextAccessor;

        private ClaimsPrincipal? User => _contextAccessor.HttpContext?.User;

        public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
        public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
    }
}
