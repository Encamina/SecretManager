using Azure.Data.Tables;
using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Infrastructure.Metadata;

public sealed class TableEnvironmentCatalog
{
    private readonly TableClient _environmentsTableClient;

    public TableEnvironmentCatalog(TableClient environmentsTableClient)
    {
        _environmentsTableClient = environmentsTableClient;
    }

    public async Task<IReadOnlyList<EnvironmentRecord>> ListByServiceAsync(
        string projectId,
        string serviceId,
        CancellationToken cancellationToken = default)
    {
        var results = new List<EnvironmentRecord>();
        var partitionKey = $"{projectId}|{serviceId}";

        await foreach (var entity in _environmentsTableClient.QueryAsync<TableEntity>(
            filter: $"{nameof(TableEntity.PartitionKey)} eq '{partitionKey}'",
            cancellationToken: cancellationToken))
        {
            results.Add(new EnvironmentRecord(
                projectId,
                serviceId,
                entity.RowKey,
                GetString(entity, "displayName") ?? entity.RowKey,
                GetBoolean(entity, "isProduction", defaultValue: false),
                GetBoolean(entity, "isActive", defaultValue: true)));
        }

        return results;
    }

    private static string? GetString(TableEntity entity, string propertyName) =>
        entity.TryGetValue(propertyName, out var value) ? value?.ToString() : null;

    private static bool GetBoolean(TableEntity entity, string propertyName, bool defaultValue) =>
        entity.TryGetValue(propertyName, out var value) && value is bool typedValue
            ? typedValue
            : defaultValue;
}
