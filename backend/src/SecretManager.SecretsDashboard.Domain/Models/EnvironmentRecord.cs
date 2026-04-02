namespace SecretManager.SecretsDashboard.Domain.Models;

public sealed record EnvironmentRecord(
    string ProjectId,
    string ServiceId,
    string EnvironmentId,
    string DisplayName,
    bool IsProduction,
    bool IsActive);
