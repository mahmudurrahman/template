using ERA.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace ERA.Modules.Multitenancy.Contracts.v1.TenantProvisioning;

public sealed record GetTenantProvisioningStatusQuery(string TenantId) : IQuery<TenantProvisioningStatusDto>;
