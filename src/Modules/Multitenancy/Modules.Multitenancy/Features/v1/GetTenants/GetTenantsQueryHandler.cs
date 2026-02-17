using ERA.Framework.Shared.Persistence;
using ERA.Modules.Multitenancy.Contracts.Dtos;
using ERA.Modules.Multitenancy.Contracts.v1.GetTenants;
using ERA.Modules.Multitenancy.Contracts;
using Mediator;

namespace ERA.Modules.Multitenancy.Features.v1.GetTenants;

public sealed class GetTenantsQueryHandler(ITenantService tenantService)
    : IQueryHandler<GetTenantsQuery, PagedResponse<TenantDto>>
{
    public async ValueTask<PagedResponse<TenantDto>> Handle(GetTenantsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        return await tenantService.GetAllAsync(query, cancellationToken).ConfigureAwait(false);
    }
}
