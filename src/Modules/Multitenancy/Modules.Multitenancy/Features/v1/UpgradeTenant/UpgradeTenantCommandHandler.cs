using ERA.Modules.Multitenancy.Contracts;
using ERA.Modules.Multitenancy.Contracts.v1.UpgradeTenant;
using Mediator;

namespace ERA.Modules.Multitenancy.Features.v1.UpgradeTenant;

public sealed class UpgradeTenantCommandHandler(ITenantService service)
    : ICommandHandler<UpgradeTenantCommand, UpgradeTenantCommandResponse>
{
    public async ValueTask<UpgradeTenantCommandResponse> Handle(UpgradeTenantCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var validUpto = await service.UpgradeSubscriptionAsync(command.Tenant, command.ExtendedExpiryDate, cancellationToken).ConfigureAwait(false);
        return new UpgradeTenantCommandResponse(validUpto, command.Tenant);
    }
}