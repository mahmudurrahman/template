using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Roles.GetRoles;

public sealed record GetRolesQuery : IQuery<IEnumerable<RoleDto>>;

