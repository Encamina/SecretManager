using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Domain.Abstractions;

public interface IAuditEventRecorder
{
    Task RecordAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);
}
