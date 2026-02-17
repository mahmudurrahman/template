using System.Security.Claims;
using ERA.Framework.Core.Exceptions;
using ERA.Framework.Shared.Identity;
using ERA.Framework.Shared.Identity.Authorization;
using ERA.Framework.Shared.Identity.Claims;
using ERA.Modules.Identity.Contracts.v1.Users.GetUserPermissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ERA.Modules.Identity.Features.v1.Users.GetUserPermissions;

public static class GetUserPermissionsEndpoint
{
    internal static RouteHandlerBuilder MapGetCurrentUserPermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/permissions", async (ClaimsPrincipal user, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }

            return await mediator.Send(new GetCurrentUserPermissionsQuery(userId), cancellationToken);
        })
        .WithName("GetCurrentUserPermissions")
        .WithSummary("Get current user permissions")
        .WithDescription("Retrieve permissions for the authenticated user.")
        .RequirePermission(IdentityPermissionConstants.Users.View);
    }
}
