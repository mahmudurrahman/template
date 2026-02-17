using ERA.Framework.Caching;
using ERA.Framework.Jobs;
using ERA.Framework.Mailing;
using ERA.Framework.Persistence;
using ERA.Framework.Web.Auth;
using ERA.Framework.Web.Cors;
using ERA.Framework.Web.Exceptions;
using ERA.Framework.Web.Health;
using ERA.Framework.Web.Mediator.Behaviors;
using ERA.Framework.Web.Modules;
using ERA.Framework.Web.Observability.Logging.Serilog;
using ERA.Framework.Web.Observability.OpenTelemetry;
using ERA.Framework.Web.OpenApi;
using ERA.Framework.Web.Origin;
using ERA.Framework.Web.RateLimiting;
using ERA.Framework.Web.Security;
using ERA.Framework.Web.Versioning;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ERA.Framework.Web;

public static class Extensions
{
    public static IHostApplicationBuilder AddHeroPlatform(this IHostApplicationBuilder builder, Action<FshPlatformOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var options = new FshPlatformOptions();
        configure?.Invoke(options);

        builder.Services.AddScoped<CurrentUserMiddleware>();

        builder.AddHeroLogging();
        if (options.EnableOpenTelemetry)
        {
            builder.AddHeroOpenTelemetry();
        }

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHeroDatabaseOptions(builder.Configuration);
        builder.Services.AddHeroRateLimiting(builder.Configuration);

        var corsEnabled = options.EnableCors && IsCorsEnabled(builder.Configuration);
        var openApiEnabled = options.EnableOpenApi && IsOpenApiEnabled(builder.Configuration);

        if (corsEnabled)
        {
            builder.Services.AddHeroCors(builder.Configuration);
        }

        builder.Services.AddHeroVersioning();

        if (openApiEnabled)
        {
            builder.Services.AddHeroOpenApi(builder.Configuration);
        }

        builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

        if (options.EnableJobs)
        {
            builder.Services.AddHeroJobs();
        }

        if (options.EnableMailing)
        {
            builder.Services.AddHeroMailing();
        }

        if (options.EnableCaching)
        {
            builder.Services.AddHeroCaching(builder.Configuration);
        }

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        builder.Services.AddProblemDetails();
        builder.Services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));
        builder.Services.AddOptions<SecurityHeadersOptions>().BindConfiguration(nameof(SecurityHeadersOptions));

        return builder;
    }


    public static WebApplication UseHeroPlatform(this WebApplication app, Action<FshPipelineOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = new FshPipelineOptions();
        configure?.Invoke(options);

        var corsEnabled = options.UseCors && IsCorsEnabled(app.Configuration);
        var openApiEnabled = options.UseOpenApi && IsOpenApiEnabled(app.Configuration);

        app.UseExceptionHandler();
        app.UseHttpsRedirection();

        app.UseHeroSecurityHeaders();

        // Serve static files as early as possible to short-circuit pipeline
        if (options.ServeStaticFiles)
        {
            var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }

            app.UseStaticFiles();
        }

        app.UseHeroJobDashboard(app.Configuration);
        app.UseRouting();

        // CORS should run between routing and authN/authZ
        if (corsEnabled)
        {
            app.UseHeroCors();
        }

        if (openApiEnabled)
        {
            app.UseHeroOpenApi();
        }

        app.UseAuthentication();

        // If Auditing module is referenced, wire its HTTP middleware (request/response logging)
        var auditMiddlewareType = Type.GetType("FSH.Modules.Auditing.AuditHttpMiddleware, FSH.Modules.Auditing");
        if (auditMiddlewareType is not null)
        {
            app.UseMiddleware(auditMiddlewareType);
        }

        app.UseHeroRateLimiting();
        app.UseAuthorization();

        if (options.MapModules)
        {
            app.MapModules();
        }

        // Always expose health endpoints
        app.MapHeroHealthEndpoints();
        app.UseMiddleware<CurrentUserMiddleware>();
        return app;
    }

    private static bool IsCorsEnabled(IConfiguration configuration)
    {
        var allowAll = configuration.GetValue("CorsOptions:AllowAll", false);
        var origins = configuration.GetSection("CorsOptions:AllowedOrigins").Get<string[]>() ?? [];
        return allowAll || origins.Length > 0;
    }

    private static bool IsOpenApiEnabled(IConfiguration configuration)
    {
        return configuration.GetValue("OpenApiOptions:Enabled", true);
    }
}

public sealed class FshPlatformOptions
{
    public bool EnableCors { get; set; } = true;
    public bool EnableOpenApi { get; set; } = true;
    public bool EnableCaching { get; set; } = false;
    public bool EnableJobs { get; set; } = false;
    public bool EnableMailing { get; set; } = false;
    public bool EnableOpenTelemetry { get; set; } = true;
}

public sealed class FshPipelineOptions
{
    public bool UseCors { get; set; } = true;
    public bool UseOpenApi { get; set; } = true;
    public bool ServeStaticFiles { get; set; } = true;
    public bool MapModules { get; set; } = true;
}
