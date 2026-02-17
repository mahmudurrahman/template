using ERA.Framework.Shared.Identity;
using ERA.Framework.Shared.Identity.Authorization;
using ERA.Modules.Identity.Contracts.v1.Roles.GetRoles;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ERA.Modules.Identity.Features.v1.Roles.GetRoles;

public static class GetRolesEndpoint
{
    public static RouteHandlerBuilder MapGetRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/roles", (IMediator mediator, CancellationToken cancellationToken) =>
            mediator.Send(new GetRolesQuery(), cancellationToken))
        .WithName("ListRoles")
        .WithSummary("List all roles")
        .RequirePermission(IdentityPermissionConstants.Roles.View)
        .WithDescription("Retrieve all roles available for the current tenant.");
    }
}
