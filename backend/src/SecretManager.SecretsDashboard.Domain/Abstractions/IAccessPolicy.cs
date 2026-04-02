using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Domain.Abstractions;

public interface IAccessPolicy
{
    bool CanAccessProject(AccessContext accessContext, string projectId);

    bool CanViewSecret(AccessContext accessContext, SecretIdentifier identifier);

    bool CanEditSecret(AccessContext accessContext, SecretIdentifier identifier);

    bool CanViewAuditTrail(AccessContext accessContext, string projectId);
}
