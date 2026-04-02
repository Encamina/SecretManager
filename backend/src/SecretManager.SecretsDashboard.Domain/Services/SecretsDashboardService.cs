using SecretManager.SecretsDashboard.Domain.Abstractions;
using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Domain.Services;

public sealed class SecretsDashboardService
{
    private readonly ISecretStore _secretStore;
    private readonly IMetadataCatalog _metadataCatalog;
    private readonly IAccessPolicy _accessPolicy;
    private readonly IAuditEventRecorder _auditEventRecorder;

    public SecretsDashboardService(
        ISecretStore secretStore,
        IMetadataCatalog metadataCatalog,
        IAccessPolicy accessPolicy,
        IAuditEventRecorder auditEventRecorder)
    {
        _secretStore = secretStore;
        _metadataCatalog = metadataCatalog;
        _accessPolicy = accessPolicy;
        _auditEventRecorder = auditEventRecorder;
    }

    public AccessContext GetCurrentUser(AccessContext accessContext)
    {
        EnsureAuthenticated(accessContext);
        return accessContext;
    }

    public SecretIdentifier CreateSecretIdentifier(
        string projectId,
        string serviceId,
        string environmentId,
        string name) =>
        new(projectId, serviceId, environmentId, name);

    public Task<IReadOnlyList<ProjectRecord>> ListProjectsAsync(CancellationToken cancellationToken = default) =>
        _metadataCatalog.ListProjectsAsync(cancellationToken);

    public void EnsureProjectAccess(AccessContext accessContext, string projectId)
    {
        EnsureAuthenticated(accessContext);

        if (!_accessPolicy.CanAccessProject(accessContext, projectId))
        {
            throw new UnauthorizedAccessException($"The current user cannot access project '{projectId}'.");
        }
    }

    public void EnsureSecretViewAccess(AccessContext accessContext, SecretIdentifier identifier)
    {
        EnsureAuthenticated(accessContext);

        if (!_accessPolicy.CanViewSecret(accessContext, identifier))
        {
            throw new UnauthorizedAccessException(
                $"The current user cannot view secret '{identifier.Name}' in '{identifier.ProjectId}/{identifier.ServiceId}/{identifier.EnvironmentId}'.");
        }
    }

    public void EnsureSecretEditAccess(AccessContext accessContext, SecretIdentifier identifier)
    {
        EnsureAuthenticated(accessContext);

        if (!_accessPolicy.CanEditSecret(accessContext, identifier))
        {
            throw new UnauthorizedAccessException(
                $"The current user cannot edit secret '{identifier.Name}' in '{identifier.ProjectId}/{identifier.ServiceId}/{identifier.EnvironmentId}'.");
        }
    }

    public Task RecordDeniedSecretViewAsync(
        AccessContext accessContext,
        SecretIdentifier identifier,
        string correlationId,
        CancellationToken cancellationToken = default) =>
        _auditEventRecorder.RecordAsync(
            AuditEvent.Create(
                AuditEventType.SecretViewed,
                AuditEventResult.Denied,
                accessContext,
                identifier,
                correlationId),
            cancellationToken);

    public Task RecordDeniedSecretEditAsync(
        AccessContext accessContext,
        SecretIdentifier identifier,
        string correlationId,
        CancellationToken cancellationToken = default) =>
        _auditEventRecorder.RecordAsync(
            AuditEvent.Create(
                AuditEventType.SecretEdited,
                AuditEventResult.Denied,
                accessContext,
                identifier,
                correlationId),
            cancellationToken);

    public Task RecordSecretEditConflictAsync(
        AccessContext accessContext,
        SecretIdentifier identifier,
        string correlationId,
        string? secretVersion,
        CancellationToken cancellationToken = default) =>
        _auditEventRecorder.RecordAsync(
            AuditEvent.Create(
                AuditEventType.SecretEdited,
                AuditEventResult.Conflict,
                accessContext,
                identifier,
                correlationId,
                secretVersion),
            cancellationToken);

    private static void EnsureAuthenticated(AccessContext accessContext)
    {
        if (accessContext.IsExpired)
        {
            throw new UnauthorizedAccessException("The current access token has expired.");
        }
    }
}
