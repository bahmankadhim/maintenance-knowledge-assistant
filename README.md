# Maintenance Knowledge Assistant

An internal tool for field maintenance teams. Engineers manage equipment and work orders today, and will soon ask questions about maintenance manuals and past reports (RAG) and get answers with the source document.

This repository currently contains the **Web API**.

## Tech stack

- ASP.NET Core Web API on **.NET 10** (controllers)
- **EF Core 10** with SQL Server (LocalDB for development)
- **JWT bearer** authentication with role-based authorization (`Technician`, `Supervisor`)
- DataAnnotations validation and **ProblemDetails** (RFC 9110) error responses
- Global exception handling with `IExceptionHandler` (unique-key violations → `409 Conflict`)
- **xUnit** integration tests with `WebApplicationFactory` and the EF Core InMemory provider

## API

| Method | Route | Access |
|---|---|---|
| `POST` | `/api/auth/login` | Anonymous (development only, demo users) |
| `GET` | `/api/equipment` | Any signed-in user |
| `GET` | `/api/equipment/{id}` | Any signed-in user |
| `POST` | `/api/equipment` | `Supervisor` |
| `GET` | `/api/workorder?equipmentId={id}` | Any signed-in user |
| `GET` | `/api/workorder/{id}` | Any signed-in user |
| `POST` | `/api/workorder` | Any signed-in user |

## Run locally

Prerequisites: .NET 10 SDK and SQL Server LocalDB.

```bash
# 1. Set the JWT signing key (kept out of source control)
dotnet user-secrets set "Jwt:Key" "<a random string of at least 32 characters>" --project src/Mka.Api

# 2. Create the database
dotnet tool install --global dotnet-ef
dotnet ef database update --project src/Mka.Api

# 3. Run the API
dotnet run --project src/Mka.Api
```

Use `src/Mka.Api/Mka.Api.http` (VS Code REST Client or Visual Studio) to log in as `tech1` or `super1` and call the endpoints.

## Tests

```bash
dotnet test
```

The tests start the whole API in memory, replace SQL Server with an in-memory database, and cover authentication (401), role checks (403), validation (400), not found (404) and successful creates (201 with a `Location` header).

## Project structure

```
src/Mka.Api
  Auth/            Token service and auth models
  Controllers/     Equipment, WorkOrder, Auth
  Data/            AppDbContext (constraints, seed data)
  Dtos/            Request/response records and mappings
  Infrastructure/  Global exception handler
  Migrations/      EF Core migrations
  Models/          Entities and enums
  Repositories/    Repository interfaces and EF implementations
tests/Mka.Api.Tests
  MkaApiFactory.cs        Test host setup
  *ApiTests.cs            Integration tests
```

## Security notes

- No secrets are stored in the repository. The JWT key comes from User Secrets locally and from App Settings / Key Vault in Azure.
- The demo login endpoint is enabled only in the Development environment. Production will use Microsoft Entra ID.

## Roadmap

- [x] Web API with EF Core, validation, error handling, JWT and tests
- [ ] Deploy to Azure App Service with Azure SQL
- [ ] CI/CD with GitHub Actions
- [ ] Document upload to Blob Storage and processing with Azure Functions
- [ ] RAG chat endpoint with source citations
- [ ] SharePoint document library via Microsoft Graph
- [ ] Power Automate flow with Teams notifications
