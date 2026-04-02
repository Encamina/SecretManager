namespace SecretManager.SecretsDashboard.Domain.Models;

public sealed record ServiceRecord(
    string ProjectId,
    string ServiceId,
    string DisplayName,
    bool IsActive);
