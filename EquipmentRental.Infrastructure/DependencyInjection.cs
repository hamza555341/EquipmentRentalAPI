using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Contracts.Identity;
using EquipmentRental.Domain.Interfaces;
using EquipmentRental.Infrastructure.Identity;
using EquipmentRental.Infrastructure.Persistence;
using EquipmentRental.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EquipmentRental.Infrastructure
{
    public static class DependencyInjection
    {
        // كل تسجيلات طبقة الـ Infrastructure: الداتابيز + UnitOfWork + التوكن + الـ Seed
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccessTokenService, AccessTokenService>();
            services.AddScoped<IDbIntializer, SeedData>();

            return services;
        }
    }
}
