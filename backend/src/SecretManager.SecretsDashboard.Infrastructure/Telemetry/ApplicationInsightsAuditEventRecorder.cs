using Microsoft.ApplicationInsights;
using SecretManager.SecretsDashboard.Domain.Abstractions;
using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Infrastructure.Telemetry;

public sealed class ApplicationInsightsAuditEventRecorder : IAuditEventRecorder
{
    private readonly TelemetryClient _telemetryClient;

    public ApplicationInsightsAuditEventRecorder(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }

    public Task RecordAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _telemetryClient.TrackEvent(
            auditEvent.EventType.ToString(),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["eventId"] = auditEvent.EventId,
                ["result"] = auditEvent.Result.ToString(),
                ["userId"] = auditEvent.UserId,
                ["projectId"] = auditEvent.ProjectId,
                ["serviceId"] = auditEvent.ServiceId,
                ["environmentId"] = auditEvent.EnvironmentId,
                ["variableName"] = auditEvent.VariableName,
                ["correlationId"] = auditEvent.CorrelationId,
                ["timestamp"] = auditEvent.Timestamp.ToString("O"),
                ["secretVersion"] = auditEvent.SecretVersion ?? string.Empty
            });

        return Task.CompletedTask;
    }
}
