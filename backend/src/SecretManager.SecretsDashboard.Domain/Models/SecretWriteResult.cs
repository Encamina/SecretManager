namespace SecretManager.SecretsDashboard.Domain.Models;

public sealed record SecretWriteResult(bool ConflictDetected, string? CurrentVersion, SecretRecord? Secret)
{
    public static SecretWriteResult Conflict(string? currentVersion) =>
        new(true, currentVersion, null);

    public static SecretWriteResult Success(SecretRecord secret) =>
        new(false, secret.Version, secret);
}
