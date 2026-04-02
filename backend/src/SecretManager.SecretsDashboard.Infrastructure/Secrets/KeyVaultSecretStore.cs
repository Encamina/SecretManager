using Azure;
using Azure.Security.KeyVault.Secrets;
using SecretManager.SecretsDashboard.Domain.Abstractions;
using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Infrastructure.Secrets;

public sealed class KeyVaultSecretStore : ISecretStore
{
    private readonly SecretClient _secretClient;

    public KeyVaultSecretStore(SecretClient secretClient)
    {
        _secretClient = secretClient;
    }

    public async Task<IReadOnlyList<SecretRecord>> ListSecretsAsync(
        string projectId,
        string serviceId,
        string environmentId,
        CancellationToken cancellationToken = default)
    {
        var results = new List<SecretRecord>();

        await foreach (var properties in _secretClient.GetPropertiesOfSecretsAsync(cancellationToken))
        {
            if (!MatchesGrouping(properties, projectId, serviceId, environmentId))
            {
                continue;
            }

            results.Add(ToSecretRecord(properties));
        }

        return results;
    }

    public async Task<SecretRecord?> GetSecretAsync(
        SecretIdentifier identifier,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _secretClient.GetSecretAsync(identifier.SecretName, cancellationToken: cancellationToken);

            if (!MatchesIdentifier(response.Value.Properties, identifier))
            {
                return null;
            }

            return ToSecretRecord(response.Value, includeValue: true);
        }
        catch (RequestFailedException exception) when (exception.Status == 404)
        {
            return null;
        }
    }

    public async Task<SecretWriteResult> SetSecretAsync(
        SecretIdentifier identifier,
        string value,
        string? expectedVersion,
        string updatedBy,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var currentSecret = await GetSecretAsync(identifier, cancellationToken);

        if (!string.IsNullOrWhiteSpace(expectedVersion)
            && !string.Equals(currentSecret?.Version, expectedVersion, StringComparison.Ordinal))
        {
            return SecretWriteResult.Conflict(currentSecret?.Version);
        }

        var secret = new KeyVaultSecret(identifier.SecretName, value)
        {
            Properties =
            {
                ContentType = currentSecret?.ContentType
            }
        };

        foreach (var tag in currentSecret?.Tags ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
        {
            secret.Properties.Tags[tag.Key] = tag.Value;
        }

        foreach (var tag in identifier.ToTags(updatedBy))
        {
            secret.Properties.Tags[tag.Key] = tag.Value;
        }

        secret.Properties.Tags["updatedAt"] = DateTimeOffset.UtcNow.ToString("O");

        var response = await _secretClient.SetSecretAsync(secret, cancellationToken);

        return SecretWriteResult.Success(ToSecretRecord(response.Value, includeValue: true));
    }

    private static bool MatchesGrouping(
        SecretProperties properties,
        string projectId,
        string serviceId,
        string environmentId) =>
        HasTag(properties, "projectId", projectId)
        && HasTag(properties, "serviceId", serviceId)
        && HasTag(properties, "environmentId", environmentId);

    private static bool MatchesIdentifier(SecretProperties properties, SecretIdentifier identifier) =>
        MatchesGrouping(properties, identifier.ProjectId, identifier.ServiceId, identifier.EnvironmentId)
        && HasTag(properties, "variableName", identifier.Name);

    private static bool HasTag(SecretProperties properties, string key, string expectedValue) =>
        properties.Tags.TryGetValue(key, out var actualValue)
        && string.Equals(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase);

    private static SecretRecord ToSecretRecord(SecretProperties properties) =>
        new(
            CreateIdentifier(properties),
            properties.Version ?? string.Empty,
            properties.UpdatedOn ?? DateTimeOffset.UtcNow,
            ContentType: properties.ContentType,
            Tags: new Dictionary<string, string>(properties.Tags, StringComparer.OrdinalIgnoreCase),
            UpdatedBy: properties.Tags.TryGetValue("updatedBy", out var updatedBy) ? updatedBy : null);

    private static SecretRecord ToSecretRecord(KeyVaultSecret secret, bool includeValue) =>
        new(
            CreateIdentifier(secret.Properties),
            secret.Properties.Version ?? string.Empty,
            secret.Properties.UpdatedOn ?? DateTimeOffset.UtcNow,
            includeValue ? secret.Value : null,
            secret.Properties.ContentType,
            new Dictionary<string, string>(secret.Properties.Tags, StringComparer.OrdinalIgnoreCase),
            secret.Properties.Tags.TryGetValue("updatedBy", out var updatedBy) ? updatedBy : null);

    private static SecretIdentifier CreateIdentifier(SecretProperties properties) =>
        new(
            properties.Tags["projectId"],
            properties.Tags["serviceId"],
            properties.Tags["environmentId"],
            properties.Tags["variableName"]);
}
