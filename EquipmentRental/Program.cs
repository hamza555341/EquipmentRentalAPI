using EquipmentRental.Api.Extensions;
using EquipmentRental.Application;
using EquipmentRental.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiServices()
    .AddSwaggerDocumentation()
    .AddAuthServices(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

await app.UseApiPipeline();

app.Run();
