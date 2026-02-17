using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Users.GetUserGroups;

public sealed record GetUserGroupsQuery(string UserId) : IQuery<IEnumerable<GroupDto>>;
