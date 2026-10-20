using EquipmentRental.Api.Helper;
using EquipmentRental.Application;
using EquipmentRental.Application.Common.Behaviors;
using EquipmentRental.Application.Common.Contracts;
using EquipmentRental.Domain.Interfaces;
using EquipmentRental.Infrastructure.Persistence;
using EquipmentRental.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<ApplicationDbContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


// إعدادات الـ Swagger الأساسية (بدون JWT Security)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();



builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
});


builder.Services.AddScoped<IDbIntializer, SeedData>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Register FluentValidation validators from the Application assembly
builder.Services.AddValidatorsFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<IDbIntializer>();

    await initializer.InitializeDataAsync();
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty; // دي بتخلي الـ Swagger يفتح مباشرة على الصفحة الرئيسية للمشروع!
        options.DisplayRequestDuration();
        options.EnableFilter();
    });
}

app.MapControllers();

app.Run();






