namespace SecretManager.SecretsDashboard.Api.Auth;

public static class RequiredRoles
{
    public const string Viewer = "SecretsDashboard.Viewer";
    public const string Editor = "SecretsDashboard.Editor";
    public const string Auditor = "SecretsDashboard.Auditor";

    public static readonly IReadOnlyCollection<string> ReadRoles = new[]
    {
        Viewer,
        Editor,
        Auditor
    };
}
