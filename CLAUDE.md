# FSH .NET Starter Kit — AI Assistant Guide

> Modular Monolith · CQRS · DDD · Multi-Tenant · .NET 10

## Quick Start

```bash
# Prerequisites: PostgreSQL running, database "fsh" created
dotnet build src/FSH.Framework.slnx                                # Build (0 warnings required)
dotnet test src/FSH.Framework.slnx                                 # Run tests
dotnet run --project src/Playground/Playground.Api                  # Run API only
dotnet run --project src/Playground/FSH.Playground.AppHost          # Run with Aspire
```

**After startup:** Open `https://localhost:7030/scalar` for API docs (not `/swagger`).

**Default login:** `admin@root.com` / `123Pa$$word!` (root tenant)

## Tech Stack

| Package | Version | Purpose |
|---------|---------|---------|
| .NET | 10.0 | Runtime & SDK |
| EF Core | 10.0.2 | ORM + migrations |
| Mediator | 3.0.1 | CQRS (source-generated, not MediatR) |
| FluentValidation | 12.1.1 | Command/query validation |
| Finbuckle.MultiTenant | 10.0.2 | Tenant resolution & isolation |
| Hangfire | 1.8.23 | Background jobs |
| Scalar | 2.12.36 | API documentation UI |
| Mapster | 7.4.0 | Object mapping |
| Serilog | 4.3.1 | Structured logging |
| PostgreSQL (Npgsql) | 10.0.0 | Primary database provider |

## Project Layout

```
src/
├── BuildingBlocks/          # Framework (11 packages) — ⚠️ Protected, do not modify
├── Modules/                 # Business features — Add code here
│   ├── Identity/            # Auth, users, roles, sessions, groups
│   │   ├── Modules.Identity.Contracts/   # Public interfaces & commands
│   │   └── Modules.Identity/             # Implementation
│   ├── Multitenancy/        # Tenant management (Finbuckle)
│   │   ├── Modules.Multitenancy.Contracts/
│   │   └── Modules.Multitenancy/
│   └── Auditing/            # Audit logging
│       ├── Modules.Auditing.Contracts/
│       └── Modules.Auditing/
├── Playground/              # Reference application
│   ├── Playground.Api/      # Host (Program.cs, appsettings)
│   ├── FSH.Playground.AppHost/  # Aspire orchestrator
│   └── Playground.Migrations.PostgreSQL/  # EF Core migrations
└── Tests/                   # Architecture + unit tests (5 projects)
```

**Module split:** Each module has a `Contracts` project (public API) and an implementation project. Modules may only reference other modules' Contracts, never their implementations.

## The Pattern

Every feature = vertical slice:

```
Modules/{Module}/Features/v1/{Feature}/
├── {Action}{Entity}Command.cs      # ICommand<T>
├── {Action}{Entity}Handler.cs      # ICommandHandler<T,R>
├── {Action}{Entity}Validator.cs    # AbstractValidator<T>
└── {Action}{Entity}Endpoint.cs     # MapPost/Get/Put/Delete
```

## Critical Rules

| Rule | Detail |
|------|--------|
| Use **Mediator** not MediatR | Source-generated library, different interfaces |
| `ICommand<T>` / `IQuery<T>` | NOT `IRequest<T>` |
| `ValueTask<T>` return type | NOT `Task<T>` on handlers |
| Every command needs a validator | `AbstractValidator<T>`, no exceptions |
| `.RequirePermission()` on endpoints | Explicit authorization required |
| Zero build warnings | CI blocks merges |
| File-scoped namespaces | Required by `.editorconfig` |
| Explicit types, no `var` | `.editorconfig` enforces `csharp_style_var_*=false` |
| `sealed` on commands, handlers, validators | Prevent unintended inheritance |
| Primary constructors preferred | For DI injection on handlers |

## Running the Application

### Prerequisites

1. **PostgreSQL** running locally
2. Create database: `CREATE DATABASE fsh;`
3. Default connection: `Server=localhost;Database=fsh;Username=postgres;Password=sqladmin123!@#`

### Run Modes

| Mode | Command | Use case |
|------|---------|----------|
| API only | `dotnet run --project src/Playground/Playground.Api` | Development |
| Aspire | `dotnet run --project src/Playground/FSH.Playground.AppHost` | Full stack with dashboard |

### Key URLs

| URL | Purpose |
|-----|---------|
| `https://localhost:7030/scalar` | API documentation (interactive) |
| `https://localhost:7030/jobs` | Hangfire dashboard (admin/Secure1234!Me) |
| `https://localhost:7030/health/live` | Liveness probe |
| `https://localhost:7030/health/ready` | Readiness probe (includes DB check) |
| `https://localhost:7030/openapi/v1.json` | OpenAPI spec |

### Default Credentials

| What | Value |
|------|-------|
| Root tenant ID | `root` |
| Admin email | `admin@root.com` |
| Admin password | `123Pa$$word!` |
| Admin username | `ROOT.ADMIN` |
| Hangfire user | `admin` |
| Hangfire password | `Secure1234!Me` |

### Migration Flow

Migrations depend on Hangfire, which requires the database to exist first. On startup:
1. `TenantStoreInitializerHostedService` seeds the root tenant
2. `TenantAutoProvisioningHostedService` provisions tenant databases (if `AutoProvisionOnStartup=true`)
3. `IdentityDbInitializer` runs per-tenant: creates roles (Admin, Basic), seeds admin user, assigns permissions

### Environment Differences

| Setting | Development | Production |
|---------|-------------|------------|
| `MultitenancyOptions:RunTenantMigrationsOnStartup` | `false` | `true` |
| `MultitenancyOptions:AutoProvisionOnStartup` | `true` | - |
| `CorsOptions:AllowAll` | `true` | `false` (explicit origins) |
| OpenTelemetry OTLP export | Disabled | Enabled |

## Configuration Reference

| Option Class | Key Properties | Defaults |
|---|---|---|
| `DatabaseOptions` | `Provider`, `ConnectionString`, `MigrationsAssembly` | `POSTGRESQL`, empty, empty |
| `JwtOptions` | `SigningKey`, `AccessTokenMinutes`, `RefreshTokenDays` | empty (32+ chars required), `2`, `7` |
| `HangfireOptions` | `UserName`, `Password`, `Route` | `admin`, `Secure1234!Me`, `/jobs` |
| `CachingOptions` | `Redis`, `KeyPrefix`, `DefaultSlidingExpiration` | empty (= in-memory), `fsh_`, 5min |
| `MultitenancyOptions` | `RunTenantMigrationsOnStartup`, `AutoProvisionOnStartup` | `false`, `true` |
| `CorsOptions` | `AllowAll`, `AllowedOrigins` | `true`, `[]` |
| `SecurityHeadersOptions` | `Enabled`, `ExcludedPaths` | `true`, `["/scalar", "/openapi"]` |
| `FshPlatformOptions` | `EnableCaching`, `EnableJobs`, `EnableMailing`, `EnableOpenTelemetry` | `false`, `false`, `false`, `true` |

**Production requires:** `DatabaseOptions:ConnectionString`, `CachingOptions:Redis`, `JwtOptions:SigningKey`

## Module Registration

Modules implement `IModule` and are discovered via `[FshModule]` attribute:

```csharp
// AssemblyInfo.cs — register module with load order
[assembly: FshModule(typeof(CatalogModule), 400)]

// Module class — DI + endpoint registration
public sealed class CatalogModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder) { /* DI */ }
    public void MapEndpoints(IEndpointRouteBuilder endpoints) { /* routes */ }
}
```

**Load order:** Identity (100) → Multitenancy (200) → Auditing (300) → your modules (400+)

**Program.cs registration:** Mediator assemblies must be explicitly listed in `builder.Services.AddMediator()`.

## Domain Base Classes

| Type | Purpose | Key Members |
|------|---------|-------------|
| `BaseEntity<TId>` | Entity with domain events | `Id`, `DomainEvents`, `AddDomainEvent()` |
| `AggregateRoot<TId>` | Aggregate root marker | Inherits `BaseEntity<TId>` |
| `IAuditableEntity` | Created/modified tracking | `CreatedOnUtc`, `CreatedBy`, `LastModifiedOnUtc`, `LastModifiedBy` |
| `IHasTenant` | Multi-tenant marker | `TenantId` |
| `ISoftDeletable` | Soft-delete support | `IsDeleted`, `DeletedOnUtc`, `DeletedBy` |
| `DomainEvent` | Base domain event record | `EventId`, `OccurredOnUtc`, `CorrelationId`, `TenantId` |

## Key Endpoints

| Module | Count | Groups |
|--------|-------|--------|
| **Identity** | 39 | Tokens, Users, Roles, Sessions, Groups |
| **Multitenancy** | 11 | CRUD, Provisioning, Themes, Migrations |
| **Auditing** | 7 | Trails, Security, Exceptions, Summary, Correlation/Trace |

All endpoints versioned under `/api/v{version}/{module}/`.

## Permission System

**Format:** `Permissions.{Resource}.{Action}` (e.g., `Permissions.Users.Create`)

**Tiers:**
- **Root** — tenant management, system-level (root tenant only)
- **Admin** — user/role management, module administration
- **Basic** — read-only access (view users, roles, audit trails)

**Usage on endpoints:**
```csharp
.RequirePermission("Permissions.Products.Create")
```

**Registering custom permissions:** Add entries to `PermissionConstants` with `FshPermission` records specifying action, resource, and tier flags (`IsRoot`, `IsBasic`).

## Testing

**Frameworks:** xUnit, Shouldly, NSubstitute, AutoFixture, NetArchTest

**5 test projects:**
1. `Architecture.Tests` — module boundaries, naming, layering (11 test classes)
2. `Identity.Tests` — handler unit tests
3. `Multitenancy.Tests` — handler unit tests
4. `Auditing.Tests` — handler unit tests
5. `Generic.Tests` — shared validation tests

**Architecture tests enforce:**
- Modules don't reference other module implementations (only Contracts)
- BuildingBlocks don't reference modules
- Every command handler has a matching validator
- Endpoints are static classes with `Map` methods in Features namespace
- Endpoints use `.RequirePermission()` for authorization
- No circular dependencies between projects

## CI/CD

**GitHub Actions** (`.github/workflows/ci.yml`):

1. **Build** — `dotnet build -c Release` (zero warnings required)
2. **Test** — parallel matrix across all 5 test projects
3. **Publish** — NuGet packages + container images (on tags/main)

## Available Skills

| Skill | Purpose |
|-------|---------|
| `/add-feature` | Create complete CQRS feature (command/handler/validator/endpoint) |
| `/add-entity` | Add domain entity with base class inheritance |
| `/add-module` | Scaffold new bounded context module |
| `/query-patterns` | Implement paginated/filtered queries |
| `/testing-guide` | Write architecture + unit tests |

## Available Agents

| Agent | Expertise |
|-------|----------|
| `code-reviewer` | Review changes against FSH patterns + architecture rules |
| `feature-scaffolder` | Generate complete feature slices from requirements |
| `module-creator` | Create new modules with contracts, persistence, DI setup |
| `architecture-guard` | Verify layering, dependencies, module boundaries |
| `migration-helper` | Generate and apply EF Core migrations |

## Example: Create Feature

```csharp
// Command
public sealed record CreateProductCommand(string Name, decimal Price)
    : ICommand<Guid>;

// Handler
public sealed class CreateProductHandler(IRepository<Product> repo)
    : ICommandHandler<CreateProductCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var product = Product.Create(cmd.Name, cmd.Price);
        await repo.AddAsync(product, ct);
        return product.Id;
    }
}

// Validator
public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

// Endpoint
public static class CreateProductEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost("/", async (CreateProductCommand cmd, IMediator mediator, CancellationToken ct) =>
            TypedResults.Created($"/api/v1/products/{await mediator.Send(cmd, ct)}"))
        .WithName(nameof(CreateProductCommand))
        .WithSummary("Create a new product")
        .RequirePermission("Permissions.Products.Create");
}
```

## Before Committing

```bash
dotnet build src/FSH.Framework.slnx  # Must pass with 0 warnings
dotnet test src/FSH.Framework.slnx   # All tests must pass
```

## Documentation

- **Architecture:** See `ARCHITECTURE_ANALYSIS.md` (19KB deep-dive)
- **Rules:** See `.claude/rules/*.md` (API conventions, testing, modules)
- **Skills:** See `.claude/skills/*/SKILL.md` (step-by-step guides)
- **Agents:** See `.claude/agents/*.md` (specialized assistants)

---

**Philosophy:** This is a production-ready starter kit. Every pattern is battle-tested. Follow the conventions, and you'll ship faster.
