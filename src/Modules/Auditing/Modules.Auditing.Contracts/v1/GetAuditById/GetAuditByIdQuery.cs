using ERA.Modules.Auditing.Contracts.Dtos;
using Mediator;

namespace ERA.Modules.Auditing.Contracts.v1.GetAuditById;

public sealed record GetAuditByIdQuery(Guid Id) : IQuery<AuditDetailDto>;

