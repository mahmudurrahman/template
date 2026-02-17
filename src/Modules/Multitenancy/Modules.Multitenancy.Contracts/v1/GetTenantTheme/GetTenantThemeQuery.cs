using ERA.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace ERA.Modules.Multitenancy.Contracts.v1.GetTenantTheme;

public sealed record GetTenantThemeQuery : IQuery<TenantThemeDto>;
