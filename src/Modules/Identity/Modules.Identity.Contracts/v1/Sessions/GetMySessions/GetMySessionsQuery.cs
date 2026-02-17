using ERA.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace ERA.Modules.Identity.Contracts.v1.Sessions.GetMySessions;

public sealed record GetMySessionsQuery : IQuery<List<UserSessionDto>>;
