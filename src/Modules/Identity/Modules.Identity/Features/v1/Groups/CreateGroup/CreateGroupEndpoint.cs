using ERA.Framework.Shared.Identity;
using ERA.Framework.Shared.Identity.Authorization;
using ERA.Modules.Identity.Contracts.v1.Groups.CreateGroup;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ERA.Modules.Identity.Features.v1.Groups.CreateGroup;

public static class CreateGroupEndpoint
{
    public static RouteHandlerBuilder MapCreateGroupEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/groups", (IMediator mediator, [FromBody] CreateGroupCommand request, CancellationToken cancellationToken) =>
            mediator.Send(request, cancellationToken))
        .WithName("CreateGroup")
        .WithSummary("Create a new group")
        .RequirePermission(IdentityPermissionConstants.Groups.Create)
        .WithDescription("Create a new group with optional role assignments.");
    }
}
