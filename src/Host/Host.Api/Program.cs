using ERA.Framework.Web;
using ERA.Framework.Web.Modules;
using ERA.Modules.Auditing;
using ERA.Modules.Identity;
using ERA.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;
using ERA.Modules.Identity.Features.v1.Tokens.TokenGeneration;
using ERA.Modules.Multitenancy;
using ERA.Modules.Multitenancy.Contracts.v1.GetTenantStatus;
using ERA.Modules.Multitenancy.Features.v1.GetTenantStatus;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    static void Require(IConfiguration config, string key)
    {
        if (string.IsNullOrWhiteSpace(config[key]))
        {
            throw new InvalidOperationException($"Missing required configuration '{key}' in Production.");
        }
    }

    var config = builder.Configuration;
    Require(config, "DatabaseOptions:ConnectionString");
    Require(config, "CachingOptions:Redis");
    Require(config, "JwtOptions:SigningKey");
}

builder.Services.AddMediator(o =>
{
    o.ServiceLifetime = ServiceLifetime.Scoped;
    o.Assemblies = [
        typeof(GenerateTokenCommand),
        typeof(GenerateTokenCommandHandler),
        typeof(GetTenantStatusQuery),
        typeof(GetTenantStatusQueryHandler),
        typeof(ERA.Modules.Auditing.Contracts.AuditEnvelope),
        typeof(ERA.Modules.Auditing.Persistence.AuditDbContext)];
});

var moduleAssemblies = new Assembly[]
{
    typeof(IdentityModule).Assembly,
    typeof(MultitenancyModule).Assembly,
    typeof(AuditingModule).Assembly
};

builder.AddHeroPlatform(o =>
{
    o.EnableCaching = true;
    o.EnableMailing = true;
    o.EnableJobs = true;
});

builder.AddModules(moduleAssemblies);
var app = builder.Build();

app.UseHeroMultiTenantDatabases();
app.UseHeroPlatform(p =>
{
    p.MapModules = true;
    p.ServeStaticFiles = true;
});

app.MapGet("/", () => Results.Ok(new { message = "hello trade finance!" }))
   .WithTags("Host")
   .AllowAnonymous();
await app.RunAsync();
