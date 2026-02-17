using ERA.Framework.Shared.Multitenancy;
using ERA.Modules.Multitenancy.Provisioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERA.Modules.Multitenancy.Data.Configurations;

public class TenantProvisioningConfiguration : IEntityTypeConfiguration<TenantProvisioning>
{
    public void Configure(EntityTypeBuilder<TenantProvisioning> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("TenantProvisionings", MultitenancyConstants.Schema);

        builder.HasMany(p => p.Steps)
            .WithOne(s => s.Provisioning!)
            .HasForeignKey(s => s.ProvisioningId);
    }
}
