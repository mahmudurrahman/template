using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Users.GetUserProfile;

public sealed record GetCurrentUserProfileQuery(string UserId) : IQuery<UserDto>;

