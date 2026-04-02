using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FunctionsHttpRequestData = Microsoft.Azure.Functions.Worker.Http.HttpRequestData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SecretManager.SecretsDashboard.Domain.Abstractions;
using SecretManager.SecretsDashboard.Domain.Models;

namespace SecretManager.SecretsDashboard.Api.Auth;

public static class EntraAuthorizationExtensions
{
    public static IServiceCollection AddSecretsDashboardAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<EntraAuthorizationOptions>()
            .Bind(configuration.GetSection("Entra"))
            .ValidateDataAnnotations()
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.TenantId)
                    && !string.IsNullOrWhiteSpace(options.ClientId)
                    && !string.IsNullOrWhiteSpace(options.Audience),
                "Entra authorization settings must include tenant, client, and audience values.");

        services.AddSingleton<IEntraTokenValidator, EntraTokenValidator>();
        services.AddSingleton<IAccessPolicy, EntraAccessPolicy>();

        return services;
    }
}

public sealed class EntraAuthorizationOptions
{
    [Required]
    public string TenantId { get; init; } = string.Empty;

    [Required]
    public string ClientId { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;
}

public interface IEntraTokenValidator
{
    Task<AccessContext> ValidateRequestAsync(FunctionsHttpRequestData request, CancellationToken cancellationToken = default);

    AccessContext CreateAccessContext(ClaimsPrincipal principal);
}

internal sealed class EntraTokenValidator : IEntraTokenValidator
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
    private readonly EntraAuthorizationOptions _options;

    public EntraTokenValidator(IOptions<EntraAuthorizationOptions> options)
    {
        _options = options.Value;

        var metadataAddress =
            $"https://login.microsoftonline.com/{_options.TenantId}/v2.0/.well-known/openid-configuration";

        _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever());
    }

    public async Task<AccessContext> ValidateRequestAsync(
        FunctionsHttpRequestData request,
        CancellationToken cancellationToken = default)
    {
        if (!request.Headers.TryGetValues("Authorization", out var authorizationValues))
        {
            throw new UnauthorizedAccessException("Missing Authorization header.");
        }

        var token = ExtractBearerToken(authorizationValues);
        var validationParameters = await CreateValidationParametersAsync(cancellationToken);
        var principal = TokenHandler.ValidateToken(token, validationParameters, out _);

        return CreateAccessContext(principal);
    }

    public AccessContext CreateAccessContext(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var userId = principal.FindFirstValue("oid")
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("The access token does not contain an object identifier.");

        var email = principal.FindFirstValue(ClaimTypes.Upn)
            ?? principal.FindFirstValue(ClaimTypes.Email)
            ?? principal.FindFirstValue("preferred_username")
            ?? throw new UnauthorizedAccessException("The access token does not contain a user principal name.");

        var expiresAt = principal.FindFirstValue("exp") is { } expiresClaim
            && long.TryParse(expiresClaim, out var expiresUnixTime)
                ? DateTimeOffset.FromUnixTimeSeconds(expiresUnixTime)
                : DateTimeOffset.UtcNow;

        return new AccessContext(
            userId,
            email,
            expiresAt,
            GetClaimValues(principal, "roles"),
            GetAllowedProjects(principal),
            principal.Identity?.Name ?? principal.FindFirstValue("name"));
    }

    private async Task<TokenValidationParameters> CreateValidationParametersAsync(CancellationToken cancellationToken)
    {
        var openIdConfiguration = await _configurationManager.GetConfigurationAsync(cancellationToken);
        var authority = $"https://login.microsoftonline.com/{_options.TenantId}/v2.0";

        return new TokenValidationParameters
        {
            ValidAudiences = new[]
            {
                _options.Audience,
                $"api://{_options.ClientId}"
            },
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuers = new[]
            {
                authority,
                $"https://sts.windows.net/{_options.TenantId}/"
            },
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = openIdConfiguration.SigningKeys,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    }

    private static string ExtractBearerToken(IEnumerable<string> authorizationValues)
    {
        var authorizationHeader = authorizationValues.FirstOrDefault(static value =>
            value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase));

        if (authorizationHeader is null)
        {
            throw new UnauthorizedAccessException("The Authorization header must use the Bearer scheme.");
        }

        var token = authorizationHeader["Bearer ".Length..].Trim();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException("The bearer token is missing.");
        }

        return token;
    }

    private static IReadOnlyCollection<string> GetAllowedProjects(ClaimsPrincipal principal)
    {
        var allowedProjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var claim in principal.FindAll("allowed_project"))
        {
            if (!string.IsNullOrWhiteSpace(claim.Value))
            {
                allowedProjects.Add(claim.Value.Trim());
            }
        }

        foreach (var claim in principal.FindAll("allowed_projects"))
        {
            foreach (var projectId in claim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                allowedProjects.Add(projectId);
            }
        }

        return allowedProjects.ToArray();
    }

    private static IReadOnlyCollection<string> GetClaimValues(ClaimsPrincipal principal, string claimType) =>
        principal.FindAll(claimType)
            .Select(claim => claim.Value)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}

internal sealed class EntraAccessPolicy : IAccessPolicy
{
    public bool CanAccessProject(AccessContext accessContext, string projectId)
    {
        ArgumentNullException.ThrowIfNull(accessContext);

        return !accessContext.IsExpired
            && accessContext.CanAccessProject(projectId)
            && accessContext.HasAnyRole(RequiredRoles.ReadRoles);
    }

    public bool CanViewSecret(AccessContext accessContext, SecretIdentifier identifier) =>
        CanAccessProject(accessContext, identifier.ProjectId);

    public bool CanEditSecret(AccessContext accessContext, SecretIdentifier identifier)
    {
        ArgumentNullException.ThrowIfNull(accessContext);

        return !accessContext.IsExpired
            && accessContext.CanAccessProject(identifier.ProjectId)
            && accessContext.HasRole(RequiredRoles.Editor);
    }

    public bool CanViewAuditTrail(AccessContext accessContext, string projectId)
    {
        ArgumentNullException.ThrowIfNull(accessContext);

        return !accessContext.IsExpired
            && accessContext.CanAccessProject(projectId)
            && accessContext.HasRole(RequiredRoles.Auditor);
    }
}
