using EquipmentRental.Api.Middlewares;
using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Application.Common.Contracts.Identity;
using EquipmentRental.Domain.Interfaces;
using EquipmentRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace EquipmentRental.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        // الـ Pipeline كله بالترتيب الصح
        public static async Task UseApiPipeline(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();   // أول Middleware دايمًا
            ConfigureSwagger(app);
            ConfigureRequestPipeline(app);
            await app.InitializeAsync();                      // Migrate + Seed
        }

        private static void ConfigureSwagger(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
                return;

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Equipment Rental API v1");
                options.RoutePrefix = string.Empty;
                options.DisplayRequestDuration();
                options.DocExpansion(DocExpansion.None);
                options.EnableFilter();
            });
        }

        private static void ConfigureRequestPipeline(WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }

        // بيعمل الداتابيز (Migrations) وبعدها بيزرع الـ Roles والـ Admin
        private static async Task InitializeAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();

            var initializer = scope.ServiceProvider.GetRequiredService<IDbIntializer>();
            await initializer.InitializeDataAsync();
        }
    }
}
