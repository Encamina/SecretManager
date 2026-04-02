using Azure.Data.Tables;
using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Infrastructure.Metadata;

public sealed class TableServiceCatalog
{
    private readonly TableClient _servicesTableClient;

    public TableServiceCatalog(TableClient servicesTableClient)
    {
        _servicesTableClient = servicesTableClient;
    }

    public async Task<IReadOnlyList<ServiceRecord>> ListByProjectAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ServiceRecord>();

        await foreach (var entity in _servicesTableClient.QueryAsync<TableEntity>(
            filter: $"{nameof(TableEntity.PartitionKey)} eq '{projectId}'",
            cancellationToken: cancellationToken))
        {
            results.Add(new ServiceRecord(
                projectId,
                entity.RowKey,
                GetString(entity, "displayName") ?? entity.RowKey,
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
