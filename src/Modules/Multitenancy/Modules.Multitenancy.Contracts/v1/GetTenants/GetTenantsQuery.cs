using ERA.Framework.Shared.Persistence;
using ERA.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace ERA.Modules.Multitenancy.Contracts.v1.GetTenants;

public sealed class GetTenantsQuery : IPagedQuery, IQuery<PagedResponse<TenantDto>>
{
    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }

    public string? Sort { get; set; }
}
