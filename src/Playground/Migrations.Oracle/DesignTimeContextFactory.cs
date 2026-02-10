using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Shared.Multitenancy;
using FSH.Framework.Shared.Persistence;
using FSH.Modules.Auditing.Persistence;
using FSH.Modules.Identity.Data;
using FSH.Modules.Multitenancy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FSH.Playground.Migrations.Oracle;

internal sealed class OracleTenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<TenantDbContext> optionsBuilder = new();
        optionsBuilder.UseOracle(
            DesignTimeHelper.ConnectionString,
            b => b.MigrationsAssembly(DesignTimeHelper.MigrationsAssembly));
        return new TenantDbContext(optionsBuilder.Options);
    }
}

internal sealed class OracleIdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<IdentityDbContext> optionsBuilder = new();
        optionsBuilder.UseOracle(
            DesignTimeHelper.ConnectionString,
            b => b.MigrationsAssembly(DesignTimeHelper.MigrationsAssembly));

        return new IdentityDbContext(
            DesignTimeHelper.CreateMultiTenantContextAccessor(),
            optionsBuilder.Options,
            Options.Create(DesignTimeHelper.DatabaseOptions),
            new DesignTimeHostEnvironment());
    }
}

internal sealed class OracleAuditDbContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
{
    public AuditDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<AuditDbContext> optionsBuilder = new();
        optionsBuilder.UseOracle(
            DesignTimeHelper.ConnectionString,
            b => b.MigrationsAssembly(DesignTimeHelper.MigrationsAssembly));

        return new AuditDbContext(
            DesignTimeHelper.CreateMultiTenantContextAccessor(),
            optionsBuilder.Options,
            Options.Create(DesignTimeHelper.DatabaseOptions),
            new DesignTimeHostEnvironment());
    }
}

internal static class DesignTimeHelper
{
    public const string ConnectionString = "User Id=fsh;Password=fsh;Data Source=localhost:1521/FREEPDB1";
    public const string MigrationsAssembly = "FSH.Playground.Migrations.Oracle";

    public static DatabaseOptions DatabaseOptions => new()
    {
        Provider = DbProviders.Oracle,
        ConnectionString = ConnectionString,
        MigrationsAssembly = MigrationsAssembly
    };

    public static IMultiTenantContextAccessor<AppTenantInfo> CreateMultiTenantContextAccessor()
    {
        MultiTenantContext<AppTenantInfo> context = new(new AppTenantInfo());
        return new StaticMultiTenantContextAccessor<AppTenantInfo>(context);
    }
}

internal sealed class StaticMultiTenantContextAccessor<T>(MultiTenantContext<T> context)
    : IMultiTenantContextAccessor<T> where T : class, ITenantInfo, new()
{
    public IMultiTenantContext<T> MultiTenantContext { get; set; } = context;

    IMultiTenantContext IMultiTenantContextAccessor.MultiTenantContext => (IMultiTenantContext)MultiTenantContext;
}

internal sealed class DesignTimeHostEnvironment : IHostEnvironment
{
    public string EnvironmentName { get; set; } = "Development";
    public string ApplicationName { get; set; } = "FSH.Playground.Migrations.Oracle";
    public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
