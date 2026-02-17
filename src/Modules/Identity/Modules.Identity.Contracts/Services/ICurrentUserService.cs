using System.Security.Claims;
using ERA.Framework.Core.Context;

namespace ERA.Modules.Identity.Contracts.Services;

/// <summary>
/// Service interface for managing the current user context.
/// Combines user identity access with initialization capabilities.
/// </summary>
public interface ICurrentUserService : ICurrentUser, ICurrentUserInitializer
{
}
