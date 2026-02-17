using ERA.Framework.Core.Exceptions;
using ERA.Framework.Shared.Identity;
using ERA.Framework.Shared.Identity.Authorization;
using ERA.Framework.Shared.Identity.Claims;
using ERA.Modules.Identity.Contracts.v1.Users.UpdateUser;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ERA.Modules.Identity.Features.v1.Users.UpdateUser;

public static class UpdateUserEndpoint
{
    internal static RouteHandlerBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/profile", async ([FromBody] UpdateUserCommand request, ClaimsPrincipal user, IMediator mediator, CancellationToken cancellationToken) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }

            request.Id = userId;

            await mediator.Send(request, cancellationToken);
            return TypedResults.Ok();
        })
        .WithName("UpdateUserProfile")
        .WithSummary("Update user profile")
        .RequirePermission(IdentityPermissionConstants.Users.Update)
        .WithDescription("Update profile details for the authenticated user.");
    }
}
