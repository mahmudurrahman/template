using FluentValidation;
using ERA.Framework.Web.Validation;
using ERA.Modules.Multitenancy.Contracts.v1.GetTenants;

namespace ERA.Modules.Multitenancy.Features.v1.GetTenants;

public sealed class GetTenantsQueryValidator : AbstractValidator<GetTenantsQuery>
{
    public GetTenantsQueryValidator()
    {
        Include(new PagedQueryValidator<GetTenantsQuery>());
    }
}
