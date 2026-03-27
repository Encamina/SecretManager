# Data Model: Secrets Management Dashboard

## Project

**Purpose**: Top-level business grouping used to organize services and their
environment variables.

**Fields**:
- `projectId` (string, required): Stable identifier used in routing and metadata.
- `displayName` (string, required): Human-readable project name.
- `description` (string, optional): Short summary shown in the dashboard.
- `isActive` (boolean, required): Whether the project is selectable.
- `partitionKey` (string, required): Azure Table Storage partition key, fixed to
  `PROJECT` for project rows.
- `rowKey` (string, required): Azure Table Storage row key, equal to `projectId`.

**Relationships**:
- One project has many services.
- One project has many audit events through its secrets.

**Validation Rules**:
- `projectId` must be unique and URL-safe.
- Inactive projects must not accept secret edits.
- Project rows in Azure Table Storage must not contain secret values.

## Service

**Purpose**: Application or component within a project.

**Fields**:
- `serviceId` (string, required): Stable identifier unique within a project.
- `projectId` (string, required): Parent project reference.
- `displayName` (string, required): Human-readable service name.
- `isActive` (boolean, required): Whether the service is selectable.
- `partitionKey` (string, required): Azure Table Storage partition key, equal to
  `projectId`.
- `rowKey` (string, required): Azure Table Storage row key, equal to `serviceId`.

**Relationships**:
- One project has many services.
- One service has many environments.
- One service has many environment variables.

**Validation Rules**:
- `serviceId` must be unique within its project.
- Services cannot exist without a parent project.
- Service rows in Azure Table Storage must not contain secret values.

## Environment

**Purpose**: Deployment context within a service, such as development, staging,
or production.

**Fields**:
- `environmentId` (string, required): Stable identifier unique within a service.
- `serviceId` (string, required): Parent service reference.
- `projectId` (string, required): Parent project reference for fast authorization.
- `displayName` (string, required): Human-readable environment name.
- `isProduction` (boolean, required): Indicates whether elevated authorization
  rules apply.
- `partitionKey` (string, required): Azure Table Storage partition key, equal to
  `projectId|serviceId`.
- `rowKey` (string, required): Azure Table Storage row key, equal to
  `environmentId`.

**Relationships**:
- One service has many environments.
- One environment has many environment variables.

**Validation Rules**:
- `environmentId` must be unique within its service.
- Production environments may require stronger edit authorization than non-prod.
- Environment rows in Azure Table Storage must not contain secret values.

## Environment Variable

**Purpose**: Secret-backed configuration item grouped by project, service, and
environment.

**Fields**:
- `name` (string, required): Variable name shown in the UI and used as the
  logical secret key.
- `projectId` (string, required): Metadata reference.
- `serviceId` (string, required): Metadata reference.
- `environmentId` (string, required): Metadata reference.
- `value` (secret string, required for view/edit operations): Plaintext value in
  transit only; durable value remains in Key Vault.
- `keyVaultSecretName` (string, required): Actual secret identifier in Key Vault.
- `version` (string, required): Current Key Vault version used for conflict checks.
- `contentType` (string, optional): Classification or usage hint.
- `tags` (map<string,string>, optional): Additional non-secret metadata for
  filtering or display.
- `updatedBy` (string, required): Last modifying user identifier.
- `updatedAt` (datetime, required): Last update timestamp.

**Relationships**:
- Each environment variable belongs to one project, one service, and one
  environment.
- Each environment variable can have many audit events and many historical secret
  versions in Key Vault.

**Validation Rules**:
- `name` must be unique within a project/service/environment grouping.
- Referenced project, service, and environment metadata must exist in Azure Table
  Storage before a secret can be viewed or edited.
- Empty values are allowed only if the business rule for the target variable
  permits them; otherwise edits must be rejected.
- Updates must include the currently viewed `version`.

**State Transitions**:
- `Current` -> `Current`: successful view operation with no state change.
- `Current` -> `Updated`: successful edit creates a new Key Vault version.
- `Current` -> `ConflictDetected`: edit attempted with stale version.
- `Current` -> `AccessDenied`: view/edit attempted without required authorization.

## Access Context

**Purpose**: Caller identity and authorization envelope derived from Microsoft
Entra ID claims and application roles.

**Fields**:
- `userId` (string, required): Entra object identifier.
- `displayName` (string, optional): Friendly user name.
- `email` (string, required): User principal name or email.
- `roles` (array<string>, required): Application roles such as viewer, editor,
  or auditor.
- `allowedProjects` (array<string>, required): Authorized project identifiers.
- `tokenExpiresAt` (datetime, required): Access-token expiry used to reject stale
  or expired sessions.

**Relationships**:
- One access context can initiate many browse/view/edit actions.

**Validation Rules**:
- API requests must be rejected if `tokenExpiresAt` is in the past.
- Edit operations require an editor-capable role.
- Audit endpoint access requires an auditor-capable role.

## Audit Event

**Purpose**: Immutable operational record of secret access or change activity.

**Fields**:
- `eventId` (string, required): Unique event identifier.
- `eventType` (enum, required): `SecretViewed` or `SecretEdited`.
- `userId` (string, required): Actor identifier.
- `projectId` (string, required): Target project.
- `serviceId` (string, required): Target service.
- `environmentId` (string, required): Target environment.
- `variableName` (string, required): Target environment variable.
- `secretVersion` (string, optional): Key Vault version involved in the action.
- `result` (enum, required): `Succeeded`, `Denied`, or `Conflict`.
- `correlationId` (string, required): Request correlation identifier.
- `timestamp` (datetime, required): Event timestamp.

**Relationships**:
- Many audit events can refer to one environment variable.
- Many audit events can be generated by one access context.

**Validation Rules**:
- Audit events must never contain plaintext secret values.
- Every view/edit attempt handled by the API must emit exactly one final result
  event.
