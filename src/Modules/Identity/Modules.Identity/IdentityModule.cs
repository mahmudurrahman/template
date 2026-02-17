using Asp.Versioning;
using ERA.Framework.Core.Context;
using ERA.Framework.Eventing;
using ERA.Framework.Eventing.Outbox;
using ERA.Modules.Identity.Features.v1.Tokens.RefreshToken;
using ERA.Modules.Identity.Features.v1.Tokens.TokenGeneration;
using ERA.Modules.Identity.Features.v1.Users.SelfRegistration;
using ERA.Framework.Persistence;
using ERA.Framework.Storage.Local;
using ERA.Framework.Storage.Services;
using ERA.Framework.Storage;
using ERA.Framework.Web.Modules;
using ERA.Modules.Identity.Authorization;
using ERA.Modules.Identity.Authorization.Jwt;
using ERA.Modules.Identity.Contracts.Services;
using ERA.Modules.Identity.Data;
using ERA.Modules.Identity.Domain;
using ERA.Modules.Identity.Features.v1.Roles;
using ERA.Modules.Identity.Features.v1.Roles.DeleteRole;
using ERA.Modules.Identity.Features.v1.Roles.GetRoleById;
using ERA.Modules.Identity.Features.v1.Roles.GetRoles;
using ERA.Modules.Identity.Features.v1.Roles.GetRoleWithPermissions;
using ERA.Modules.Identity.Features.v1.Roles.UpdateRolePermissions;
using ERA.Modules.Identity.Features.v1.Roles.UpsertRole;
using ERA.Modules.Identity.Features.v1.Users.AssignUserRoles;
using ERA.Modules.Identity.Features.v1.Users.ChangePassword;
using ERA.Modules.Identity.Features.v1.Users.ConfirmEmail;
using ERA.Modules.Identity.Features.v1.Users.DeleteUser;
using ERA.Modules.Identity.Features.v1.Users.GetUserById;
using ERA.Modules.Identity.Features.v1.Users.GetUserPermissions;
using ERA.Modules.Identity.Features.v1.Users.GetUserProfile;
using ERA.Modules.Identity.Features.v1.Users.GetUserRoles;
using ERA.Modules.Identity.Features.v1.Users.GetUsers;
using ERA.Modules.Identity.Features.v1.Users.RegisterUser;
using ERA.Modules.Identity.Features.v1.Users.SearchUsers;
using ERA.Modules.Identity.Features.v1.Users.ResetPassword;
using ERA.Modules.Identity.Features.v1.Users.ToggleUserStatus;
using ERA.Modules.Identity.Features.v1.Users.UpdateUser;
using ERA.Modules.Identity.Features.v1.Sessions.GetMySessions;
using ERA.Modules.Identity.Features.v1.Sessions.RevokeSession;
using ERA.Modules.Identity.Features.v1.Sessions.RevokeAllSessions;
using ERA.Modules.Identity.Features.v1.Sessions.GetUserSessions;
using ERA.Modules.Identity.Features.v1.Sessions.AdminRevokeSession;
using ERA.Modules.Identity.Features.v1.Sessions.AdminRevokeAllSessions;
using ERA.Modules.Identity.Features.v1.Groups.CreateGroup;
using ERA.Modules.Identity.Features.v1.Groups.UpdateGroup;
using ERA.Modules.Identity.Features.v1.Groups.DeleteGroup;
using ERA.Modules.Identity.Features.v1.Groups.GetGroups;
using ERA.Modules.Identity.Features.v1.Groups.GetGroupById;
using ERA.Modules.Identity.Features.v1.Groups.GetGroupMembers;
using ERA.Modules.Identity.Features.v1.Groups.AddUsersToGroup;
using ERA.Modules.Identity.Features.v1.Groups.RemoveUserFromGroup;
using ERA.Modules.Identity.Features.v1.Users.GetUserGroups;
using ERA.Modules.Identity.Services;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ERA.Modules.Identity;

public class IdentityModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var services = builder.Services;
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, PathAwareAuthorizationHandler>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<ICurrentUserService>());
        services.AddScoped<ICurrentUserInitializer>(sp => sp.GetRequiredService<ICurrentUserService>());
        services.AddScoped<IRequestContextService, RequestContextService>();
        services.AddScoped<IRequestContext>(sp => sp.GetRequiredService<IRequestContextService>());
        services.AddScoped<ITokenService, TokenService>();

        // User services - focused single-responsibility services
        services.AddTransient<IUserRegistrationService, UserRegistrationService>();
        services.AddTransient<IUserProfileService, UserProfileService>();
        services.AddTransient<IUserStatusService, UserStatusService>();
        services.AddTransient<IUserRoleService, UserRoleService>();
        services.AddTransient<IUserPasswordService, UserPasswordService>();
        services.AddTransient<IUserPermissionService, UserPermissionService>();

        // Facade for backward compatibility
        services.AddTransient<IUserService, UserService>();

        services.AddTransient<IRoleService, RoleService>();
        services.AddHeroStorage(builder.Configuration);
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddHeroDbContext<IdentityDbContext>();
        services.AddEventingCore(builder.Configuration);
        services.AddEventingForDbContext<IdentityDbContext>();
        services.AddIntegrationEventHandlers(typeof(IdentityModule).Assembly);
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<IdentityDbContext>(
                name: "db:identity",
                failureStatus: HealthStatus.Unhealthy);
        services.AddScoped<IDbInitializer, IdentityDbInitializer>();

        // Configure password policy options
        services.Configure<PasswordPolicyOptions>(builder.Configuration.GetSection("PasswordPolicy"));

        // Register password history service
        services.AddScoped<IPasswordHistoryService, PasswordHistoryService>();

        // Register password expiry service
        services.AddScoped<IPasswordExpiryService, PasswordExpiryService>();

        // Register session service and background cleanup
        services.AddScoped<ISessionService, SessionService>();
        services.AddHostedService<SessionCleanupHostedService>();

        // Register group role service for group-derived permissions
        services.AddScoped<IGroupRoleService, GroupRoleService>();

        services.AddIdentity<FshUser, FshRole>(options =>
        {
            options.Password.RequiredLength = IdentityModuleConstants.PasswordLength;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        })
           .AddEntityFrameworkStores<IdentityDbContext>()
           .AddDefaultTokenProviders();

        //metrics
        services.AddSingleton<IdentityMetrics>();

        services.ConfigureJwtAuth();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var apiVersionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints
            .MapGroup("api/v{version:apiVersion}/identity")
            .WithTags("Identity")
            .WithApiVersionSet(apiVersionSet);

        // tokens
        group.MapGenerateTokenEndpoint().AllowAnonymous().RequireRateLimiting("auth");
        group.MapRefreshTokenEndpoint().AllowAnonymous().RequireRateLimiting("auth");

        // example Hangfire setup for Identity outbox dispatcher
        var jobManager = endpoints.ServiceProvider.GetService<IRecurringJobManager>();
        if (jobManager is not null)
        {
            jobManager.AddOrUpdate(
                "identity-outbox-dispatcher",
                Job.FromExpression<OutboxDispatcher>(d => d.DispatchAsync(CancellationToken.None)),
                Cron.Minutely(),
                new RecurringJobOptions());
        }

        // roles
        group.MapGetRolesEndpoint();
        group.MapGetRoleByIdEndpoint();
        group.MapDeleteRoleEndpoint();
        group.MapGetRolePermissionsEndpoint();
        group.MapUpdateRolePermissionsEndpoint();
        group.MapCreateOrUpdateRoleEndpoint();

        // users
        group.MapAssignUserRolesEndpoint();
        group.MapChangePasswordEndpoint();
        group.MapConfirmEmailEndpoint().RequireRateLimiting("auth");
        group.MapDeleteUserEndpoint();
        group.MapGetUserByIdEndpoint();
        group.MapGetCurrentUserPermissionsEndpoint();
        group.MapGetMeEndpoint();
        group.MapGetUserRolesEndpoint();
        group.MapGetUsersListEndpoint();
        group.MapSearchUsersEndpoint();
        group.MapRegisterUserEndpoint();
        group.MapResetPasswordEndpoint();
        group.MapSelfRegisterUserEndpoint();
        group.MapToggleUserStatusEndpoint();
        group.MapUpdateUserEndpoint();

        // sessions - user endpoints
        group.MapGetMySessionsEndpoint();
        group.MapRevokeSessionEndpoint();
        group.MapRevokeAllSessionsEndpoint();

        // sessions - admin endpoints
        group.MapGetUserSessionsEndpoint();
        group.MapAdminRevokeSessionEndpoint();
        group.MapAdminRevokeAllSessionsEndpoint();

        // groups
        group.MapGetGroupsEndpoint();
        group.MapGetGroupByIdEndpoint();
        group.MapCreateGroupEndpoint();
        group.MapUpdateGroupEndpoint();
        group.MapDeleteGroupEndpoint();
        group.MapGetGroupMembersEndpoint();
        group.MapAddUsersToGroupEndpoint();
        group.MapRemoveUserFromGroupEndpoint();

        // user groups
        group.MapGetUserGroupsEndpoint();
    }
}
