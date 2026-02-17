using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Identity.EntityFrameworkCore;
using ERA.Framework.Eventing.Inbox;
using ERA.Framework.Eventing.Outbox;
using ERA.Framework.Persistence;
using ERA.Framework.Shared.Multitenancy;
using ERA.Framework.Shared.Persistence;
using ERA.Modules.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ERA.Modules.Identity.Data;

public class IdentityDbContext : MultiTenantIdentityDbContext<FshUser,
    FshRole,
    string,
    IdentityUserClaim<string>,
    IdentityUserRole<string>,
    IdentityUserLogin<string>,
    FshRoleClaim,
    IdentityUserToken<string>,
    IdentityUserPasskey<string>>
{
    private readonly DatabaseOptions _settings;
    private new AppTenantInfo TenantInfo { get; set; }
    private readonly IHostEnvironment _environment;
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();

    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public DbSet<Group> Groups => Set<Group>();

    public DbSet<GroupRole> GroupRoles => Set<GroupRole>();

    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    public IdentityDbContext(
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        DbContextOptions<IdentityDbContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment) : base(multiTenantContextAccessor, options)
    {
        ArgumentNullException.ThrowIfNull(multiTenantContextAccessor);
        ArgumentNullException.ThrowIfNull(settings);

        _environment = environment;
        _settings = settings.Value;
        TenantInfo = multiTenantContextAccessor.MultiTenantContext.TenantInfo!;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        builder.ApplyConfiguration(new OutboxMessageConfiguration(IdentityModuleConstants.SchemaName));
        builder.ApplyConfiguration(new InboxMessageConfiguration(IdentityModuleConstants.SchemaName));
        builder.ApplyOracleConventions(this);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrWhiteSpace(TenantInfo?.ConnectionString))
        {
            optionsBuilder.ConfigureHeroDatabase(
                _settings.Provider,
                TenantInfo.ConnectionString,
                _settings.MigrationsAssembly,
                _environment.IsDevelopment());
        }
    }
}
