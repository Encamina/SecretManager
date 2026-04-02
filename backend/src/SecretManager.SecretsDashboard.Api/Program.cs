using Azure.Data.Tables;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecretManager.SecretsDashboard.Api.Auth;
using SecretManager.SecretsDashboard.Domain.Abstractions;
using SecretManager.SecretsDashboard.Domain.Services;
using SecretManager.SecretsDashboard.Infrastructure.Metadata;
using SecretManager.SecretsDashboard.Infrastructure.Secrets;
using SecretManager.SecretsDashboard.Infrastructure.Telemetry;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();

        services.AddSecretsDashboardAuthorization(context.Configuration);

        services.AddSingleton(new DefaultAzureCredential());
        services.AddSingleton(static sp => CreateSecretClient(
            sp.GetRequiredService<IConfiguration>(),
            sp.GetRequiredService<DefaultAzureCredential>()));
        services.AddSingleton(static sp => CreateTableServiceClient(
            sp.GetRequiredService<IConfiguration>(),
            sp.GetRequiredService<DefaultAzureCredential>()));

        services.AddSingleton(static sp => new TableServiceCatalog(CreateTableClient(
            sp.GetRequiredService<TableServiceClient>(),
            sp.GetRequiredService<IConfiguration>(),
            "Metadata:ServicesTableName")));
        services.AddSingleton(static sp => new TableEnvironmentCatalog(CreateTableClient(
            sp.GetRequiredService<TableServiceClient>(),
            sp.GetRequiredService<IConfiguration>(),
            "Metadata:EnvironmentsTableName")));
        services.AddSingleton(static sp => new TableProjectCatalog(
            CreateTableClient(
                sp.GetRequiredService<TableServiceClient>(),
                sp.GetRequiredService<IConfiguration>(),
                "Metadata:ProjectsTableName"),
            sp.GetRequiredService<TableServiceCatalog>(),
            sp.GetRequiredService<TableEnvironmentCatalog>()));

        services.AddSingleton<ISecretStore, KeyVaultSecretStore>();
        services.AddSingleton<IMetadataCatalog, TableProjectCatalog>();
        services.AddSingleton<IAuditEventRecorder, ApplicationInsightsAuditEventRecorder>();
        services.AddSingleton<SecretsDashboardService>();
    })
    .ConfigureLogging(logging =>
    {
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule? defaultRule = options.Rules.FirstOrDefault(rule =>
                rule.ProviderName == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    })
    .Build();

host.Run();

static SecretClient CreateSecretClient(IConfiguration configuration, DefaultAzureCredential credential)
{
    var vaultUri = configuration["KeyVault:VaultUri"]
        ?? throw new InvalidOperationException("Missing required configuration value 'KeyVault:VaultUri'.");

    return new SecretClient(new Uri(vaultUri), credential);
}

static TableServiceClient CreateTableServiceClient(IConfiguration configuration, DefaultAzureCredential credential)
{
    var tableServiceUri = configuration["Metadata:TableServiceUri"]
        ?? throw new InvalidOperationException("Missing required configuration value 'Metadata:TableServiceUri'.");

    return new TableServiceClient(new Uri(tableServiceUri), credential);
}

static TableClient CreateTableClient(
    TableServiceClient tableServiceClient,
    IConfiguration configuration,
    string configurationKey)
{
    var tableName = configuration[configurationKey]
        ?? throw new InvalidOperationException($"Missing required configuration value '{configurationKey}'.");

    return tableServiceClient.GetTableClient(tableName);
}
