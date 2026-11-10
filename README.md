# Maintenance Knowledge Assistant

An internal tool for field maintenance teams. Engineers manage equipment and work orders, and upload maintenance manuals and past reports. The next step is a chat that answers questions from those documents (RAG) and cites the source.

The repository contains the **Web API** and an **Azure Function** for document processing, deployed to Azure by GitHub Actions.

## Architecture

```
GitHub Actions ──(OIDC)──► Azure
                            │
  Client ── JWT ──► App Service (Linux): ASP.NET Core Web API (.NET 10)
                            │                 │
                            │                 └─ Managed Identity ──► Blob Storage (documents)
                            │                                               │ blob trigger
                            └─ EF Core ──► Azure SQL (serverless)            ▼
                                                              Azure Function (.NET 10 isolated)
```

## Tech stack

- ASP.NET Core Web API on **.NET 10** (controllers)
- **EF Core 10** with SQL Server: LocalDB for development, **Azure SQL (serverless)** in Azure, with retry-on-failure for transient errors and optional migrations on startup
- **JWT bearer** authentication with role-based authorization (`Technician`, `Supervisor`)
- DataAnnotations validation and **ProblemDetails** (RFC 9110) error responses
- Global exception handling with `IExceptionHandler` (unique-key violations → `409 Conflict`)
- **Azure Blob Storage** for document uploads, accessed with a **system-assigned managed identity** and RBAC (no storage account keys)
- **Azure Functions** (.NET 10 isolated worker) with a blob trigger that processes each new document
- **xUnit** integration tests with `WebApplicationFactory` and the EF Core InMemory provider
- **GitHub Actions** CI/CD: build, test, publish, then deploy the API and the Function with **OIDC federated credentials** (no Azure secrets stored in GitHub)

## API

| Method | Route | Access |
|---|---|---|
| `POST` | `/api/auth/login` | Anonymous, only when `Auth:EnableDemoLogin` is `true` (demo users) |
| `GET` | `/api/equipment` | Any signed-in user |
| `GET` | `/api/equipment/{id}` | Any signed-in user |
| `POST` | `/api/equipment` | `Supervisor` |
| `GET` | `/api/workorder?equipmentId={id}` | Any signed-in user |
| `GET` | `/api/workorder/{id}` | Any signed-in user |
| `POST` | `/api/workorder` | Any signed-in user |
| `GET` | `/api/document` | Any signed-in user |
| `POST` | `/api/document` | `Supervisor` (multipart upload: .pdf, .txt, .md, .docx, max 10 MB) |

## Deployment

Every push to `main` runs [.github/workflows/deploy-api.yml](.github/workflows/deploy-api.yml):

1. **build-test**: restore, build and run the tests in Release, then publish the API and the Function as artifacts.
2. **deploy** (environment `dev`): sign in to Azure with OIDC (`azure/login`), deploy the API to App Service and the Function to its Function App.

Azure resources:

| Resource | Purpose |
|---|---|
| App Service (Linux) | Hosts the Web API |
| Azure SQL Database (serverless) | Equipment and work orders |
| Storage account, `documents` container | Uploaded documents |
| Function App | Runs `ProcessDocument` for each new blob |
| Managed identities + RBAC role assignments | API and Function access storage without keys |

Configuration lives in App Service and Function App settings, never in the repository: the SQL connection string, `Jwt:Key`, `Storage:BlobEndpoint` and `Database:MigrateOnStartup`.

## Run locally

Prerequisites: .NET 10 SDK, SQL Server LocalDB, and the Azure CLI for document uploads.

```bash
# 1. Set the JWT signing key (kept out of source control)
dotnet user-secrets set "Jwt:Key" "<a random string of at least 32 characters>" --project src/Mka.Api

# 2. Create the database
dotnet tool install --global dotnet-ef
dotnet ef database update --project src/Mka.Api

# 3. Sign in to Azure (the API uses your az login for Blob Storage in Development)
az login

# 4. Run the API
dotnet run --project src/Mka.Api
```

Use `src/Mka.Api/Mka.Api.http` (VS Code REST Client or Visual Studio) to call the endpoints. Demo login is off by default; enable it locally with `Auth:EnableDemoLogin` in `appsettings.Development.json` or User Secrets.

## Tests

```bash
dotnet test
```

The tests start the whole API in memory, replace SQL Server with an in-memory database, and cover authentication (401), role checks (403), validation (400), not found (404) and successful creates (201 with a `Location` header).

## Project structure

```
src/Mka.Api
  Auth/            Token service and auth models
  Controllers/     Equipment, WorkOrder, Document, Auth
  Data/            AppDbContext (constraints, seed data)
  Dtos/            Request/response records and mappings
  Infrastructure/  Global exception handler
  Migrations/      EF Core migrations
  Models/          Entities and enums
  Repositories/    Repository interfaces and EF implementations
src/Mka.Functions
  ProcessDocument.cs      Blob-triggered function for new documents
tests/Mka.Api.Tests
  MkaApiFactory.cs        Test host setup
  *ApiTests.cs            Integration tests
.github/workflows
  deploy-api.yml          CI/CD to Azure
```

## Security notes

- No secrets are stored in the repository. The JWT key comes from User Secrets locally and from App Settings in Azure.
- Azure access from GitHub uses OIDC federated credentials, so there is no client secret to leak or rotate.
- Storage is accessed with managed identities and RBAC instead of account keys.
- The demo login endpoint returns `404` unless `Auth:EnableDemoLogin` is enabled. Production will use Microsoft Entra ID.

## Roadmap

- [x] Web API with EF Core, validation, error handling, JWT and tests
- [x] Deploy to Azure App Service with Azure SQL
- [x] CI/CD with GitHub Actions
- [x] Document upload to Blob Storage and processing with Azure Functions
- [ ] RAG chat endpoint with source citations
- [ ] SharePoint document library via Microsoft Graph
- [ ] Power Automate flow with Teams notifications
