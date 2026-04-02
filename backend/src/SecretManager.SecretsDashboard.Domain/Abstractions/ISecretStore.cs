using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Domain.Abstractions;

public interface ISecretStore
{
    Task<IReadOnlyList<SecretRecord>> ListSecretsAsync(
        string projectId,
        string serviceId,
        string environmentId,
        CancellationToken cancellationToken = default);

    Task<SecretRecord?> GetSecretAsync(
        SecretIdentifier identifier,
        CancellationToken cancellationToken = default);

    Task<SecretWriteResult> SetSecretAsync(
        SecretIdentifier identifier,
        string value,
        string? expectedVersion,
        string updatedBy,
        CancellationToken cancellationToken = default);
}
