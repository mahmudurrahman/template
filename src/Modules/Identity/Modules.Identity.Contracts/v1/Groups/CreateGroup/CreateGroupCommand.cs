using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Groups.CreateGroup;

public sealed record CreateGroupCommand(
    string Name,
    string? Description,
    bool IsDefault,
    List<string>? RoleIds) : ICommand<GroupDto>;
