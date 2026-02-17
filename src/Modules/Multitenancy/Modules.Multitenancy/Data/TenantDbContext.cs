using Finbuckle.MultiTenant.EntityFrameworkCore.Stores;
using ERA.Framework.Persistence;
using ERA.Framework.Shared.Multitenancy;
using ERA.Modules.Multitenancy.Domain;
using ERA.Modules.Multitenancy.Provisioning;
using Microsoft.EntityFrameworkCore;

namespace ERA.Modules.Multitenancy.Data;

public class TenantDbContext : EFCoreStoreDbContext<AppTenantInfo>
{
    public const string Schema = "tenant";

    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options)
    {
    }

    public DbSet<TenantProvisioning> TenantProvisionings => Set<TenantProvisioning>();

    public DbSet<TenantProvisioningStep> TenantProvisioningSteps => Set<TenantProvisioningStep>();

    public DbSet<TenantTheme> TenantThemes => Set<TenantTheme>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
        modelBuilder.ApplyOracleConventions(this);
    }
}
