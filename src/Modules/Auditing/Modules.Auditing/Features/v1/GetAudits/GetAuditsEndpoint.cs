using ERA.Framework.Shared.Identity;
using ERA.Framework.Shared.Identity.Authorization;
using ERA.Modules.Auditing.Contracts.v1.GetAudits;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ERA.Modules.Auditing.Features.v1.GetAudits;

public static class GetAuditsEndpoint
{
    public static RouteHandlerBuilder MapGetAuditsEndpoint(this IEndpointRouteBuilder group)
    {
        return group.MapGet(
                "/",
                async ([AsParameters] GetAuditsQuery query, IMediator mediator, CancellationToken cancellationToken) =>
                    await mediator.Send(query, cancellationToken))
            .WithName("GetAudits")
            .WithSummary("List and search audit events")
            .WithDescription("Retrieve audit events with pagination and filters.")
            .RequirePermission(AuditingPermissionConstants.View);
    }
}

