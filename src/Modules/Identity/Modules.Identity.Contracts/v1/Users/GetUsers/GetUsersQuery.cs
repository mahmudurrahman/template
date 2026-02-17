using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Users.GetUsers;

public sealed record GetUsersQuery : IQuery<List<UserDto>>;

