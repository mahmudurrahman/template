using ERA.Modules.Multitenancy.Contracts;
using ERA.Modules.Multitenancy.Contracts.Dtos;
using ERA.Modules.Multitenancy.Contracts.v1.GetTenantTheme;
using Mediator;

namespace ERA.Modules.Multitenancy.Features.v1.GetTenantTheme;

public sealed class GetTenantThemeQueryHandler(ITenantThemeService themeService)
    : IQueryHandler<GetTenantThemeQuery, TenantThemeDto>
{
    public async ValueTask<TenantThemeDto> Handle(GetTenantThemeQuery query, CancellationToken cancellationToken)
    {
        return await themeService.GetCurrentTenantThemeAsync(cancellationToken);
    }
}
