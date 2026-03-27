# Quickstart: Secrets Management Dashboard

## Prerequisites

- .NET 10 SDK
- Azure Functions Core Tools compatible with the isolated worker runtime
- Node.js LTS for the React frontend
- Access to an Azure subscription with Azure Static Web Apps, Azure Functions,
  Azure Table Storage, Application Insights, and Azure Key Vault
- Microsoft Entra ID tenant admin support for app registrations, API scopes, and
  application-role assignments

## 1. Provision Azure resources

1. Create an Azure Key Vault for secret storage.
2. Create a Storage Account with Azure Table Storage enabled for metadata.
3. Create an Azure Functions app for the API backend.
4. Enable a system-assigned Managed Identity on the Functions app.
5. Grant the Managed Identity `get`, `list`, and `set` access to Key Vault.
6. Grant the Managed Identity table data contributor access to the metadata tables.
7. Create an Azure Static Web App for the React frontend.
8. Connect Application Insights to the Functions app for operational events.

## 2. Configure Microsoft Entra ID

1. Register the frontend SPA application.
2. Register the backend API application and expose an API scope for the frontend.
3. Define application roles for at least viewer, editor, and auditor access.
4. Assign users or groups to the roles required for their environments.
5. Configure the frontend to request the API scope after sign-in.

## 3. Configure metadata storage

1. Create `Projects`, `Services`, and `Environments` tables in the storage account.
2. Seed project rows keyed by `projectId`.
3. Seed service rows keyed by `projectId` + `serviceId`.
4. Seed environment rows keyed by `projectId|serviceId` + `environmentId`.
5. Verify no table entity contains secret values.

## 4. Configure application settings

### Backend settings

- `Entra__TenantId`
- `Entra__ClientId`
- `Entra__Audience`
- `KeyVault__VaultUri`
- `Metadata__TableServiceUri`
- `Metadata__ProjectsTableName`
- `Metadata__ServicesTableName`
- `Metadata__EnvironmentsTableName`
- `ApplicationInsights__ConnectionString`

### Frontend settings

- `VITE_ENTRA_CLIENT_ID`
- `VITE_ENTRA_TENANT_ID`
- `VITE_API_BASE_URL`
- `VITE_ENTRA_SCOPE`

## 5. Implement and run locally

1. Build the backend domain, infrastructure, and API projects.
2. Run backend tests with `dotnet test`.
3. Start the Functions API locally with `func start` from the backend API project.
4. Install frontend dependencies and run frontend tests with `npm test`.
5. Start the frontend locally with `npm run dev`.

## 6. Validate the feature

1. Sign in with a Microsoft Entra ID account that has the viewer or editor role.
2. Browse to a project, service, and environment grouping sourced from Azure Table
   Storage.
3. Open an environment variable and confirm the value is displayed only after
   authentication and authorization succeed.
4. Edit a variable while supplying the latest version and confirm a new version is
   created in Azure Key Vault.
5. Repeat the edit using a stale version and confirm the API returns a conflict.
6. Review Application Insights events to confirm view/edit activity is recorded
   without plaintext secret values.
