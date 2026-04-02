using Azure.Data.Tables;
using SecretManager.SecretsDashboard.Domain.Abstractions;
using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Infrastructure.Metadata;

public sealed class TableProjectCatalog : IMetadataCatalog
{
    private readonly TableClient _projectsTableClient;
    private readonly TableServiceCatalog _serviceCatalog;
    private readonly TableEnvironmentCatalog _environmentCatalog;

    public TableProjectCatalog(
        TableClient projectsTableClient,
        TableServiceCatalog serviceCatalog,
        TableEnvironmentCatalog environmentCatalog)
    {
        _projectsTableClient = projectsTableClient;
        _serviceCatalog = serviceCatalog;
        _environmentCatalog = environmentCatalog;
    }

    public async Task<IReadOnlyList<ProjectRecord>> ListProjectsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<ProjectRecord>();

        await foreach (var entity in _projectsTableClient.QueryAsync<TableEntity>(
            filter: $"{nameof(TableEntity.PartitionKey)} eq 'PROJECT'",
            cancellationToken: cancellationToken))
        {
            results.Add(new ProjectRecord(
                entity.RowKey,
                GetString(entity, "displayName") ?? entity.RowKey,
                GetString(entity, "description"),
                GetBoolean(entity, "isActive", defaultValue: true)));
        }

        return results;
    }

    public Task<IReadOnlyList<ServiceRecord>> ListServicesAsync(
        string projectId,
        CancellationToken cancellationToken = default) =>
        _serviceCatalog.ListByProjectAsync(projectId, cancellationToken);

    public Task<IReadOnlyList<EnvironmentRecord>> ListEnvironmentsAsync(
        string projectId,
        string serviceId,
        CancellationToken cancellationToken = default) =>
        _environmentCatalog.ListByServiceAsync(projectId, serviceId, cancellationToken);

    private static string? GetString(TableEntity entity, string propertyName) =>
        entity.TryGetValue(propertyName, out var value) ? value?.ToString() : null;

    private static bool GetBoolean(TableEntity entity, string propertyName, bool defaultValue) =>
        entity.TryGetValue(propertyName, out var value) && value is bool typedValue
            ? typedValue
            : defaultValue;
}
