using ERA.Modules.Identity.Contracts.DTOs;
using ERA.Modules.Identity.Contracts.Services;
using ERA.Modules.Identity.Contracts.v1.Roles.GetRoles;
using Mediator;

namespace ERA.Modules.Identity.Features.v1.Roles.GetRoles;

public sealed class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, IEnumerable<RoleDto>>
{
    private readonly IRoleService _roleService;

    public GetRolesQueryHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async ValueTask<IEnumerable<RoleDto>> Handle(GetRolesQuery query, CancellationToken cancellationToken)
    {
        return await _roleService.GetRolesAsync(cancellationToken).ConfigureAwait(false);
    }
}
