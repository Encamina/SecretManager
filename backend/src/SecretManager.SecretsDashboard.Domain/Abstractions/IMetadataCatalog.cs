using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Domain.Abstractions;

public interface IMetadataCatalog
{
    Task<IReadOnlyList<ProjectRecord>> ListProjectsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ServiceRecord>> ListServicesAsync(
        string projectId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EnvironmentRecord>> ListEnvironmentsAsync(
        string projectId,
        string serviceId,
        CancellationToken cancellationToken = default);
}
