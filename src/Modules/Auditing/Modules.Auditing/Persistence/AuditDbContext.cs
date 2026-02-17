using Finbuckle.MultiTenant.Abstractions;
using ERA.Framework.Persistence;
using ERA.Framework.Persistence.Context;
using ERA.Framework.Shared.Multitenancy;
using ERA.Framework.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ERA.Modules.Auditing.Persistence;

public sealed class AuditDbContext : BaseDbContext
{
    public AuditDbContext(
    IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
    DbContextOptions<AuditDbContext> options,
    IOptions<DatabaseOptions> settings,
    IHostEnvironment environment) : base(multiTenantContextAccessor, options, settings, environment) { }

    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditDbContext).Assembly);
        modelBuilder.ApplyOracleConventions(this);
    }
}
