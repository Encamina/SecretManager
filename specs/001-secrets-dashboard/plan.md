# Implementation Plan: Secrets Management Dashboard

**Branch**: `001-secrets-dashboard` | **Date**: 2026-03-27 | **Spec**: [`spec.md`](./spec.md)
**Input**: Feature specification from `/specs/001-secrets-dashboard/spec.md`

**Note**: This plan implements the constitution requirements for encryption-before-storage,
Microsoft Entra ID-only authentication, and library-first delivery.

## Summary

Build a secrets management dashboard that lets developers browse, view, and edit
environment variables by project, service, and environment. The solution uses a
React + TypeScript frontend on Azure Static Web Apps, a .NET 10 Azure Functions
isolated-worker API, Azure Key Vault as the only secret-value store, Azure Table
Storage for project/service/environment metadata, Microsoft Entra ID for user
authentication, and a dedicated backend domain library to keep grouping, access
checks, conflict detection, and secret operations independently testable.

## Technical Context

**Language/Version**: C# on .NET 10 for the backend; TypeScript for the frontend
**Primary Dependencies**: Azure Functions Isolated Worker, Azure SDK for Key Vault,
Azure.Data.Tables, Microsoft.Identity.Web, MSAL.js, React.js, xUnit, Moq, Jest,
React Testing Library
**Storage**: Azure Key Vault for secret values; Azure Table Storage for project,
service, and environment metadata; Application Insights custom events for access
and change audit trails without plaintext values
**Testing**: xUnit + Moq for backend domain/API tests; Jest + React Testing Library
for frontend component and interaction tests
**Target Platform**: Azure Functions for the API and Azure Static Web Apps for the SPA
**Project Type**: Web application with a library-first backend core
**Performance Goals**: Browse lists render within 2 seconds for common groupings;
view and edit requests complete within 3 seconds under normal internal usage
**Constraints**: No plaintext secret persistence; Entra ID-only authentication;
Key Vault is the system of record for secret values; Azure Table Storage is the
system of record for project/service/environment metadata; fail closed if Key Vault,
Table Storage, or token validation is unavailable for the relevant operation
**Scale/Scope**: Single-tenant internal developer tool supporting dozens of
projects, hundreds of services, and thousands of environment variables

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Initial Gate Review**

- **Encryption-before-storage**: PASS. Secret values persist only in Azure Key
  Vault, which is the only durable secret store for secret material. Azure Table
  Storage stores metadata only and must never contain plaintext secret values.
- **Microsoft Entra ID-only authentication**: PASS. The frontend authenticates
  with Microsoft Entra ID via MSAL.js and the API accepts only Entra-issued
  access tokens validated through Microsoft.Identity.Web configuration.
- **Library-first delivery**: PASS. Core behavior lives in a backend domain
  library that is tested independently from the Functions HTTP triggers and the
  React frontend.
- **Operational safety evidence**: PASS. The design requires backend tests for
  grouping retrieval, conflict detection, metadata lookups, and audit recording,
  plus frontend tests for guarded access flows and secret-handling UX.

**Post-Design Re-Check**

- **Encryption-before-storage**: PASS. The contract routes all secret read and
  update operations through the API into Key Vault. Azure Table Storage stores only
  browse metadata and references, so no alternate plaintext persistence path is
  introduced.
- **Microsoft Entra ID-only authentication**: PASS. `GET /api/me` and all secret
  endpoints require Entra-authenticated identity and role claims; no fallback auth
  path exists.
- **Library-first delivery**: PASS. The planned structure separates
  `SecretManager.SecretsDashboard.Domain` from API and frontend adapters, and the
  quickstart validates the feature through domain/API tests before UI polish.
- **Operational safety evidence**: PASS. The design includes audit event capture,
  metadata validation, conflict handling, and tests that ensure plaintext values are
  not logged or stored outside Key Vault.

## Project Structure

### Documentation (this feature)

```text
specs/001-secrets-dashboard/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── secrets-dashboard-api.yaml
└── tasks.md
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── SecretManager.SecretsDashboard.Api/
│   ├── SecretManager.SecretsDashboard.Domain/
│   └── SecretManager.SecretsDashboard.Infrastructure/
└── tests/
│   ├── SecretManager.SecretsDashboard.Domain.Tests/
│   └── SecretManager.SecretsDashboard.Api.Tests/

frontend/
├── src/
│   ├── app/
│   ├── components/
│   ├── features/secrets-dashboard/
│   ├── hooks/
│   └── services/
└── tests/
```

**Structure Decision**: Use a web-application layout with a dedicated backend
domain library to satisfy the constitution's library-first rule. The API project
hosts Azure Functions triggers and token validation, the infrastructure project
wraps Key Vault, Azure Table Storage, and telemetry adapters, and the frontend
consumes the HTTP contract from Azure Static Web Apps.

## Complexity Tracking

No constitutional violations or special complexity exemptions are required for this
design.
