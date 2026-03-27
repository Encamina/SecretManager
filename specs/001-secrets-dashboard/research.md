# Research: Secrets Management Dashboard

## Decision: Overall architecture shape

**Decision**: Use a four-part architecture: React SPA -> Azure Functions API ->
Azure Table Storage for metadata + Azure Key Vault for secret values.

**Rationale**: This keeps secret handling behind the API, stores secret values only
in Key Vault, and uses a simple, query-friendly metadata store for project,
service, and environment navigation without mixing plaintext secrets into metadata
persistence.

**Alternatives considered**:
- Direct frontend access to Key Vault: rejected because it would broaden the blast
  radius of user tokens and weaken backend policy enforcement.
- Storing all metadata only in Key Vault tags: rejected because browsing and
  filtering project/service/environment hierarchies is simpler and more explicit in
  Azure Table Storage.

## Decision: Authentication and authorization flow

**Decision**: Authenticate users in the frontend with MSAL.js against Microsoft
Entra ID and require Entra-issued bearer tokens on every API request. Validate
tokens and role claims in the Functions app using Microsoft.Identity.Web-aligned
configuration.

**Rationale**: This satisfies the Entra-only constitutional rule, avoids local
credentials, and keeps authorization based on Entra claims and application roles.

**Alternatives considered**:
- Local username/password auth: rejected because it violates the constitution.
- API keys or shared service credentials: rejected because they bypass user-level
  identity and auditability.

## Decision: Secret storage and encryption boundary

**Decision**: Store secret values only in Azure Key Vault. Store project, service,
and environment metadata in Azure Table Storage. Let the API hold plaintext values
only in request memory during view/edit operations.

**Rationale**: Azure Key Vault provides encryption at rest, versioned secret
history, and managed access control for secret material. Azure Table Storage offers
inexpensive, flexible key/value-style persistence for hierarchical metadata without
introducing plaintext secret storage.

**Alternatives considered**:
- Database plus envelope encryption for both secrets and metadata: rejected for the
  initial delivery because it adds operational and key-management complexity.
- Encoding all metadata into Key Vault secret names and tags: rejected because it
  makes browse queries and metadata management harder to evolve.

## Decision: Metadata storage shape

**Decision**: Model metadata with separate Azure Table Storage tables for Projects,
Services, and Environments, each keyed for predictable lookup by project and parent
relationships.

**Rationale**: Separate tables keep ownership and activation rules clear, support
straightforward browse APIs, and avoid cross-entity overloading in a single table.

**Alternatives considered**:
- Single polymorphic metadata table: rejected because it complicates validation and
  read patterns.
- Blob or JSON document storage: rejected because point reads and filtered listings
  are a better fit for Table Storage.

## Decision: Backend library boundary

**Decision**: Put grouping, authorization checks, metadata resolution,
browse/view/edit orchestration, and optimistic conflict detection in
`SecretManager.SecretsDashboard.Domain`.

**Rationale**: This creates an independently testable core that can be exercised
without HTTP, UI, or Azure resources and matches the library-first constitutional
requirement.

**Alternatives considered**:
- Placing all logic directly in Azure Functions handlers: rejected because it
  would couple domain rules to transport concerns and reduce testability.
- Making the frontend responsible for grouping or conflict logic: rejected because
  policy must remain server-side.

## Decision: Browse and view contract style

**Decision**: Expose a REST + JSON contract for current user context, metadata
navigation, secret lists, secret details, secret updates, and audit event access.

**Rationale**: REST is a good fit for React, keeps adapters thin, and is easy to
validate with contract and integration tests.

**Alternatives considered**:
- GraphQL: rejected for the initial release because the workflow is narrow and the
  extra schema/tooling complexity is not necessary.
- Direct server-rendered pages: rejected because the chosen frontend is a React SPA.

## Decision: Concurrency and overwrite protection

**Decision**: Use optimistic concurrency based on the current Key Vault secret
version. Every edit request includes the version the user last viewed; mismatches
return a conflict response instead of silently overwriting.

**Rationale**: This satisfies the feature requirement to warn users about stale
edits without introducing server-side locks that do not fit a stateless Functions
backend.

**Alternatives considered**:
- Last-write-wins: rejected because it allows silent overwrites.
- Server-side locking: rejected because it adds coordination complexity and weakens
  horizontal scalability.

## Decision: Audit and operational event recording

**Decision**: Record secret-view and secret-edit events as Application Insights
custom events containing identity, target secret metadata, action type, result,
and correlation identifiers, but never plaintext secret values.

**Rationale**: This provides searchable operational evidence with minimal extra
infrastructure while meeting the requirement to record access and change events
safely.

**Alternatives considered**:
- Separate audit database: deferred until retention or reporting needs exceed what
  Application Insights can provide.
- Logging plaintext diffs for convenience: rejected because it violates the
  constitution.

## Decision: Testing strategy

**Decision**: Use a test pyramid with xUnit + Moq for backend domain/API tests and
Jest + React Testing Library for frontend tests.

**Rationale**: The backend library can be validated independently from adapters,
while the frontend tests can verify authentication gating, metadata navigation,
and edit flows with mocked API responses.

**Alternatives considered**:
- End-to-end-only testing: rejected because it would be slower and weaker at
  validating the library-first rule.
- Manual-only UI testing: rejected because it does not provide reliable regression
  coverage.

## Decision: Hosting and deployment shape

**Decision**: Host the frontend on Azure Static Web Apps and the backend on Azure
Functions (Isolated Worker), with system-assigned Managed Identity access from the
API to Azure Key Vault and Azure Table Storage for metadata access.

**Rationale**: This matches the requested Azure-native architecture, minimizes
operational overhead, and avoids storing static credentials for either secret or
metadata services.

**Alternatives considered**:
- Azure App Service for both frontend and backend: acceptable but less aligned
  with the requested Static Web Apps deployment model.
- Containers or Kubernetes: rejected for the initial release because the scale and
  workflow do not justify the extra operational complexity.

## Open assumptions adopted for planning

- Microsoft Entra ID app registrations, API scopes, and application roles will be
  provisioned before environment setup begins.
- The initial release is single-tenant and intended for internal developer use.
- Azure Table Storage contains only metadata and never secret values.
- Application Insights retention is sufficient for the initial audit history needs.
