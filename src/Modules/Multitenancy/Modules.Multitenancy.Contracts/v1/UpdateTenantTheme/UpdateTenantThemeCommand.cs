using ERA.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace ERA.Modules.Multitenancy.Contracts.v1.UpdateTenantTheme;

public sealed record UpdateTenantThemeCommand(TenantThemeDto Theme) : ICommand;
