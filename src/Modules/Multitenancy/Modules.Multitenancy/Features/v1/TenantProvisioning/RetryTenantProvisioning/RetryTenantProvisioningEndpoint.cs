using ERA.Framework.Shared.Identity.Authorization;
using ERA.Framework.Shared.Multitenancy;
using ERA.Modules.Multitenancy.Contracts.v1.TenantProvisioning;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ERA.Modules.Multitenancy.Features.v1.TenantProvisioning.RetryTenantProvisioning;

public static class RetryTenantProvisioningEndpoint
{
    public static RouteHandlerBuilder Map(IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{tenantId}/provisioning/retry", async (
            [FromRoute] string tenantId,
            [FromServices] IMediator mediator) =>
            await mediator.Send(new RetryTenantProvisioningCommand(tenantId)))
            .WithName("RetryTenantProvisioning")
            .WithSummary("Retry tenant provisioning")
            .RequirePermission(MultitenancyConstants.Permissions.Update)
            .WithDescription("Retry the provisioning workflow for a tenant.");
    }
}
