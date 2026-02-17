namespace ERA.Framework.Shared.Multitenancy;

public interface IAppTenantInfo
{
    string? ConnectionString { get; set; }
}