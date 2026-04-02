namespace SecretManager.SecretsDashboard.Domain.Models;

public sealed record ProjectRecord(
    string ProjectId,
    string DisplayName,
    string? Description,
    bool IsActive);
