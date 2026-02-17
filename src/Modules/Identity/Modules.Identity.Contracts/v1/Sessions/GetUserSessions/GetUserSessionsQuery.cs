using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Sessions.GetUserSessions;

public sealed record GetUserSessionsQuery(Guid UserId) : IQuery<List<UserSessionDto>>;
