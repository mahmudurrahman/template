using ERA.Framework.Shared.Identity;
using ERA.Framework.Shared.Identity.Authorization;
using ERA.Modules.Identity.Contracts.v1.Users.RegisterUser;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ERA.Modules.Identity.Features.v1.Users.RegisterUser;

public static class RegisterUserEndpoint
{
    internal static RouteHandlerBuilder MapRegisterUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/register", (RegisterUserCommand command,
            HttpContext context,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
            command.Origin = origin;
            return mediator.Send(command, cancellationToken);
        })
        .WithName("RegisterUser")
        .WithSummary("Register user")
        .RequirePermission(IdentityPermissionConstants.Users.Create)
        .WithDescription("Create a new user account.");
    }
}
