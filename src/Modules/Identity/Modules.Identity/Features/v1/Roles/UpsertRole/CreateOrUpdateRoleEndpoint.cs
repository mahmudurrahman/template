using ERA.Framework.Shared.Identity;
using ERA.Framework.Shared.Identity.Authorization;
using ERA.Modules.Identity.Contracts.v1.Roles.UpsertRole;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ERA.Modules.Identity.Features.v1.Roles.UpsertRole;

public static class CreateOrUpdateRoleEndpoint
{
    public static RouteHandlerBuilder MapCreateOrUpdateRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/roles", (IMediator mediator, [FromBody] UpsertRoleCommand request, CancellationToken cancellationToken) =>
            mediator.Send(request, cancellationToken))
        .WithName("CreateOrUpdateRole")
        .WithSummary("Create or update role")
        .RequirePermission(IdentityPermissionConstants.Roles.Create)
        .WithDescription("Create a new role or update an existing role's name and description.");
    }
}
