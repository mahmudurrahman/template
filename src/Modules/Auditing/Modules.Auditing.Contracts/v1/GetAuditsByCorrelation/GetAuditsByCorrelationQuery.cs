using ERA.Modules.Auditing.Contracts.Dtos;
using Mediator;

namespace ERA.Modules.Auditing.Contracts.v1.GetAuditsByCorrelation;

public sealed class GetAuditsByCorrelationQuery : IQuery<IReadOnlyList<AuditSummaryDto>>
{
    public string CorrelationId { get; init; } = default!;

    public DateTime? FromUtc { get; init; }

    public DateTime? ToUtc { get; init; }
}

