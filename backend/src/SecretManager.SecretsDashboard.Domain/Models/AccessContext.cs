namespace SecretManager.SecretsDashboard.Domain.Models;

public sealed record AccessContext
{
    public AccessContext(
        string userId,
        string email,
        DateTimeOffset tokenExpiresAt,
        IEnumerable<string>? roles = null,
        IEnumerable<string>? allowedProjects = null,
        string? displayName = null)
    {
        UserId = string.IsNullOrWhiteSpace(userId)
            ? throw new ArgumentException("A user identifier is required.", nameof(userId))
            : userId;
        Email = string.IsNullOrWhiteSpace(email)
            ? throw new ArgumentException("An email address is required.", nameof(email))
            : email;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName;
        TokenExpiresAt = tokenExpiresAt;
        Roles = NormalizeValues(roles);
        AllowedProjects = NormalizeValues(allowedProjects);
    }

    public string UserId { get; }

    public string? DisplayName { get; }

    public string Email { get; }

    public IReadOnlyCollection<string> Roles { get; }

    public IReadOnlyCollection<string> AllowedProjects { get; }

    public DateTimeOffset TokenExpiresAt { get; }

    public bool IsExpired => TokenExpiresAt <= DateTimeOffset.UtcNow;

    public bool HasRole(string role) =>
        Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

    public bool HasAnyRole(IEnumerable<string> roles) =>
        roles.Any(HasRole);

    public bool CanAccessProject(string projectId) =>
        AllowedProjects.Count == 0
        || AllowedProjects.Contains(projectId, StringComparer.OrdinalIgnoreCase);

    private static IReadOnlyCollection<string> NormalizeValues(IEnumerable<string>? values) =>
        values?
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Select(static value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? Array.Empty<string>();
}
