using ERA.Framework.Shared.Identity;
using ERA.Framework.Shared.Identity.Authorization;
using ERA.Modules.Identity.Contracts.v1.Sessions.GetUserSessions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ERA.Modules.Identity.Features.v1.Sessions.GetUserSessions;

public static class GetUserSessionsEndpoint
{
    internal static RouteHandlerBuilder MapGetUserSessionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/users/{userId:guid}/sessions", (Guid userId, CancellationToken cancellationToken, IMediator mediator) =>
            mediator.Send(new GetUserSessionsQuery(userId), cancellationToken))
        .WithName("GetUserSessions")
        .WithSummary("Get user's sessions (Admin)")
        .RequirePermission(IdentityPermissionConstants.Sessions.ViewAll)
        .WithDescription("Retrieve all active sessions for a specific user. Requires admin permission.");
    }
}
