using FluentValidation;
using ERA.Modules.Identity.Contracts.v1.Roles.UpdatePermissions;

namespace ERA.Modules.Identity.Features.v1.Roles.UpdateRolePermissions;

public sealed class UpdatePermissionsCommandValidator : AbstractValidator<UpdatePermissionsCommand>
{
    public UpdatePermissionsCommandValidator()
    {
        RuleFor(r => r.RoleId)
            .NotEmpty();
        RuleFor(r => r.Permissions)
            .NotNull();
    }
}