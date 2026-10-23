
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy project files first to improve restore caching
COPY EquipmentRental.Domain/EquipmentRental.Domain.csproj EquipmentRental.Domain/
COPY EquipmentRental.Application/EquipmentRental.Application.csproj EquipmentRental.Application/
COPY EquipmentRental.Infrastructure/EquipmentRental.Infrastructure.csproj EquipmentRental.Infrastructure/
COPY EquipmentRental/EquipmentRental.Api.csproj EquipmentRental/

# Restore dependencies
RUN dotnet restore EquipmentRental/EquipmentRental.Api.csproj

# Copy source code
COPY EquipmentRental.Domain/ EquipmentRental.Domain/
COPY EquipmentRental.Application/ EquipmentRental.Application/
COPY EquipmentRental.Infrastructure/ EquipmentRental.Infrastructure/
COPY EquipmentRental/ EquipmentRental/

# Publish the API
RUN dotnet publish EquipmentRental/EquipmentRental.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "EquipmentRental.Api.dll"]
