# Equipment Rental API

A backend project built with **ASP.NET Core Web API** to manage equipment rentals, borrowing, returns, and overdue loans.

The main goal of this project was not to build as many features as possible, but to focus on learning and applying backend engineering concepts such as **Clean Architecture, CQRS, Unit Testing, and Docker** in a practical project.

**Version:** 1.0.0 — Initial Release

This is the first version of the project. Additional features and improvements are planned for future releases.

## Overview

The API provides a foundation for managing equipment and tracking the borrowing lifecycle, with separate access levels for administrators and members.

### Core Features

- Equipment management and status tracking.
- Borrowing and returning equipment.
- Loan tracking and overdue loan monitoring.
- Role-based access for Admin and Member.
- API documentation through Swagger/OpenAPI.

## Architecture & Engineering Practices

- **Clean Architecture** — separation of concerns across Domain, Application, Infrastructure, and API layers.
- **CQRS with MediatR** — separates commands that change state from queries that retrieve data.
- **Repository & Unit of Work Patterns** — abstracts data access and coordinates persistence operations.
- **Result Pattern** — provides a consistent way to represent operation success and failure.
- **Global Exception Handling Middleware** — centralizes handling of unexpected exceptions.
- **Dependency Injection** — organizes application service registration.
- **Entity Framework Core & SQL Server** — database access and persistence.
- **xUnit Unit Testing** — tests application handlers and business scenarios.
- **Docker & Docker Compose** — runs the API and SQL Server in separate containers.
- **Multi-stage Docker Build** — separates build and runtime environments.
- **Automatic Database Migrations** — applies pending migrations during application startup.

## Tech Stack

- C#
- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- SQL Server
- MediatR
- Clean Architecture
- CQRS
- xUnit
- Docker & Docker Compose
- Swagger / OpenAPI

## Project Structure

```text
EquipmentRentalAPI/
├── EquipmentRental/
│   ├── Controllers/
│   ├── Middlewares/
│   ├── Extensions/
│   └── Program.cs
├── EquipmentRental.Application/
├── EquipmentRental.Domain/
│   └── Exceptions/
├── EquipmentRental.Infrastructure/
├── EquipmentRental.UnitTests/
├── Dockerfile
└── README.md
```

## Run with Docker Compose

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Docker Compose

### 1. Clone the repository

```bash
git clone https://github.com/hamza555341/EquipmentRentalAPI.git
cd EquipmentRentalAPI/EquipmentRental
```

### 2. Configure the required environment variables

Review the Compose configuration and provide any required environment variables or local configuration values before starting the services. Do not commit passwords, JWT secrets, or other credentials to the repository.

### 3. Start the application

```bash
docker compose up -d
```

Docker Compose starts the API and SQL Server containers. The application applies pending database migrations and initializes its seed data during startup.

### 4. Access the API

Once the containers are running, open:

**API / Swagger UI:** http://localhost:8080

### 5. View logs

```bash
docker compose logs -f api
```

### 6. Stop the application

```bash
docker compose stop
```

To remove the containers when you no longer need them:

```bash
docker compose down
```

The database volume is retained unless explicitly removed.

## Run Unit Tests

From the repository root, run:

```bash
dotnet test EquipmentRental.UnitTests/EquipmentRental.UnitTests.csproj
```

## Docker Hub

The published Docker image is available here:

[**Equipment Rental API — Docker Hub**](https://hub.docker.com/r/hamzaoraby/equipment-rental-api)

Pull the published image:

```bash
docker pull hamzaoraby/equipment-rental-api:1.0.0
```

To run the complete application, use Docker Compose so the API can connect to SQL Server with the required configuration.

## Future Improvements

This is the initial release, with further improvements planned as the project evolves. Future updates may include additional features, enhanced testing, and further infrastructure improvements.

## Author

**Hamza Oraby**

Backend .NET Developer

[GitHub](https://github.com/hamza555341)
