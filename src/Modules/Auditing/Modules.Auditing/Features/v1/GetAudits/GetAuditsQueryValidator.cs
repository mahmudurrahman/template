using FluentValidation;
using ERA.Framework.Web.Validation;
using ERA.Modules.Auditing.Contracts.v1.GetAudits;

namespace ERA.Modules.Auditing.Features.v1.GetAudits;

public sealed class GetAuditsQueryValidator : AbstractValidator<GetAuditsQuery>
{
    public GetAuditsQueryValidator()
    {
        Include(new PagedQueryValidator<GetAuditsQuery>());

        RuleFor(q => q)
            .Must(q => !q.FromUtc.HasValue || !q.ToUtc.HasValue || q.FromUtc <= q.ToUtc)
            .WithMessage("FromUtc must be less than or equal to ToUtc.");
    }
}

