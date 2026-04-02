namespace SecretManager.SecretsDashboard.Domain.Models;

public sealed record SecretRecord(
    SecretIdentifier Identifier,
    string Version,
    DateTimeOffset UpdatedAt,
    string? Value = null,
    string? ContentType = null,
    IReadOnlyDictionary<string, string>? Tags = null,
    string? UpdatedBy = null);
