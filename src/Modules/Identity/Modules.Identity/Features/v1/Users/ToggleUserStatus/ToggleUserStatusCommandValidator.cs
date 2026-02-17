using FluentValidation;
using ERA.Modules.Identity.Contracts.v1.Users.ToggleUserStatus;

namespace ERA.Modules.Identity.Features.v1.Users.ToggleUserStatus;

public sealed class ToggleUserStatusCommandValidator : AbstractValidator<ToggleUserStatusCommand>
{
    public ToggleUserStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
