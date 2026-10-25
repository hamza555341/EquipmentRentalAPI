using EquipmentRental.Api.Helper;
using EquipmentRental.Application.Common.Contracts;

namespace EquipmentRental.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // كل اللي يخص طبقة الـ API نفسها: Controllers + المستخدم الحالي
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
