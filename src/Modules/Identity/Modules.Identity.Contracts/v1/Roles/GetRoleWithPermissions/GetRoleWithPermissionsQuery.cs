using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Roles.GetRoleWithPermissions;

public sealed record GetRoleWithPermissionsQuery(string Id) : IQuery<RoleDto>;

