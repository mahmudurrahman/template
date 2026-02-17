using ERA.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace ERA.Modules.Multitenancy.Contracts.v1.GetTenantMigrations;

public sealed record GetTenantMigrationsQuery : IQuery<IReadOnlyCollection<TenantMigrationStatusDto>>;

