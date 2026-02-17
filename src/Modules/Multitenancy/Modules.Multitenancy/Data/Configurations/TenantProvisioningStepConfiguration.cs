using ERA.Framework.Shared.Multitenancy;
using ERA.Modules.Multitenancy.Provisioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERA.Modules.Multitenancy.Data.Configurations;

public class TenantProvisioningStepConfiguration : IEntityTypeConfiguration<TenantProvisioningStep>
{
    public void Configure(EntityTypeBuilder<TenantProvisioningStep> builder)
    {
        builder.ToTable("TenantProvisioningSteps", MultitenancyConstants.Schema);
    }
}
