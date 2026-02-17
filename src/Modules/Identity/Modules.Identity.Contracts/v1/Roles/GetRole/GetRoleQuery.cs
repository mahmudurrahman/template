using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Roles.GetRole;

public sealed record GetRoleQuery(string Id) : IQuery<RoleDto?>;

