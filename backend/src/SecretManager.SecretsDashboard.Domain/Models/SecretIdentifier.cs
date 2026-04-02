using System.Text;

namespace SecretManager.SecretsDashboard.Domain.Models;

public sealed record SecretIdentifier
{
    public SecretIdentifier(string projectId, string serviceId, string environmentId, string name)
    {
        ProjectId = RequireValue(projectId, nameof(projectId));
        ServiceId = RequireValue(serviceId, nameof(serviceId));
        EnvironmentId = RequireValue(environmentId, nameof(environmentId));
        Name = RequireValue(name, nameof(name));
    }

    public string ProjectId { get; }

    public string ServiceId { get; }

    public string EnvironmentId { get; }

    public string Name { get; }

    public string SecretName => NormalizeSecretName($"{ProjectId}--{ServiceId}--{EnvironmentId}--{Name}");

    public IReadOnlyDictionary<string, string> ToTags(string? updatedBy = null)
    {
        var tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["projectId"] = ProjectId,
            ["serviceId"] = ServiceId,
            ["environmentId"] = EnvironmentId,
            ["variableName"] = Name
        };

        if (!string.IsNullOrWhiteSpace(updatedBy))
        {
            tags["updatedBy"] = updatedBy;
        }

        return tags;
    }

    private static string RequireValue(string value, string parameterName) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A non-empty value is required.", parameterName)
            : value.Trim();

    private static string NormalizeSecretName(string value)
    {
        var builder = new StringBuilder(value.Length);
        var previousWasDash = false;

        foreach (var character in value)
        {
            var normalizedCharacter = char.IsLetterOrDigit(character) ? char.ToLowerInvariant(character) : '-';

            if (normalizedCharacter == '-')
            {
                if (previousWasDash)
                {
                    continue;
                }

                previousWasDash = true;
                builder.Append(normalizedCharacter);
                continue;
            }

            previousWasDash = false;
            builder.Append(normalizedCharacter);
        }

        return builder.ToString().Trim('-');
    }
}
