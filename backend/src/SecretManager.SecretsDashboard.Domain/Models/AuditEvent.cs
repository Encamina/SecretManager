namespace SecretManager.SecretsDashboard.Domain.Models;

public enum AuditEventType
{
    SecretViewed,
    SecretEdited
}

public enum AuditEventResult
{
    Succeeded,
    Denied,
    Conflict
}

public sealed record AuditEvent(
    string EventId,
    AuditEventType EventType,
    string UserId,
    string ProjectId,
    string ServiceId,
    string EnvironmentId,
    string VariableName,
    AuditEventResult Result,
    string CorrelationId,
    DateTimeOffset Timestamp,
    string? SecretVersion = null)
{
    public static AuditEvent Create(
        AuditEventType eventType,
        AuditEventResult result,
        AccessContext accessContext,
        SecretIdentifier identifier,
        string correlationId,
        string? secretVersion = null) =>
        new(
            Guid.NewGuid().ToString("N"),
            eventType,
            accessContext.UserId,
            identifier.ProjectId,
            identifier.ServiceId,
            identifier.EnvironmentId,
            identifier.Name,
            result,
            correlationId,
            DateTimeOffset.UtcNow,
            secretVersion);
}
