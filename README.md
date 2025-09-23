# FullstackBackend (API) - DDD with JWT (InMemory for testing)

This is a minimal **.NET 8** Web API sample following DDD + CQRS (MediatR), using EF Core InMemory provider and JWT authentication.
It implements user registration, listing, and authentication endpoints required by the test specification.

## Projects
- `src/Api` - ASP.NET Core Web API (Program.cs, Controllers)
- `src/Application` - Application layer (DTOs, Commands/Queries, Handlers)
- `src/Domain` - Domain entities and repository interfaces
- `src/Infrastructure` - EF Core DbContext and repository implementations (InMemory)
- `tests/Application.Tests` - Minimal unit test for CreateUserHandler (xUnit + Moq)

## How to build & run
Requires .NET 8 SDK installed.

From repository root:

```bash
cd src/Api
dotnet restore
dotnet build
dotnet run
```

API will start on `http://localhost:5000` (or as configured). Swagger UI is enabled.

There is a `Dockerfile` in `src/Api` with basic build instructions.

## Notes
- Persistence currently uses EF Core InMemory provider (configured in `Infrastructure`).
- JWT settings are in `appsettings.json`. For demo use the secret provided or replace it.
- The project is intentionally compact but structured for clarity and scalability.

