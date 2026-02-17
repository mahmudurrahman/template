using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Groups.GetGroupMembers;

public sealed record GetGroupMembersQuery(Guid GroupId) : IQuery<IEnumerable<GroupMemberDto>>;
