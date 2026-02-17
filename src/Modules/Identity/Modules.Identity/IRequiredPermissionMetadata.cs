namespace ERA.Modules.Identity;

public interface IRequiredPermissionMetadata
{
    HashSet<string> RequiredPermissions { get; }
}
