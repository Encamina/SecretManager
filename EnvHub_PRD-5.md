# EnvHub — Product Requirements Document

> **Version:** 1.4 | **Date:** March 27, 2026 | **Author:** Alberto Diaz | **Status:** Draft
> **Application:** [env-hub-nu.vercel.app](https://env-hub-nu.vercel.app/)

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Problem Statement](#2-problem-statement)
3. [Product Overview](#3-product-overview)
4. [Target Users](#4-target-users)
5. [Goals & Success Metrics](#5-goals--success-metrics)
6. [Functional Requirements](#6-functional-requirements)
7. [Non-Functional Requirements](#7-non-functional-requirements)
8. [User Stories](#8-user-stories)
9. [Deployment & Self-Hosting](#9-deployment--self-hosting)
10. [Security Model](#10-security-model)
11. [Product Roadmap](#11-product-roadmap)
12. [Competitive Landscape](#12-competitive-landscape)
13. [Constraints & Assumptions](#13-constraints--assumptions)
14. [Glossary](#14-glossary)
15. [Document History](#15-document-history)

---

## 1. Executive Summary

EnvHub is an open-source, zero-knowledge platform designed to help software development teams securely store, version, and manage environment variables and application secrets across multiple projects and environments. Built on a fully Azure-native architecture — a **React.js** single-page application and **.NET 10 Azure Functions** API hosted together on **Azure Static Web Apps** — with **Azure Key Vault** as the secrets storage backend, EnvHub provides a Git-inspired workflow for secrets management — eliminating the risky practice of sharing `.env` files over chat, email, or version control.

The platform combines a web-based dashboard with a powerful cross-platform CLI tool, enabling teams to push, pull, and audit environment variables with full version history and change attribution. Secrets are stored natively in Azure Key Vault, leveraging its enterprise-grade encryption, FIPS 140-2 Level 2 HSM protection, and built-in access policies. Access is controlled exclusively through **Microsoft Entra ID (formerly Azure Active Directory)** using OpenID Connect, enabling seamless SSO with existing corporate Microsoft 365 identities.

> **Core Value Proposition:** EnvHub treats environment variables like first-class code — versioned, auditable, encrypted, and team-accessible — without requiring any dedicated infrastructure to operate.

---

## 2. Problem Statement

### 2.1 Background

Environment variables and application secrets are a critical part of every software project. They contain database credentials, API keys, OAuth tokens, feature flags, and service configuration values. Yet, despite their sensitivity, most teams handle them in ways that are insecure, unversioned, and operationally fragile.

### 2.2 Current Pain Points

- **Insecure sharing:** Teams share `.env` files over Slack, email, or Google Drive, exposing secrets to potential interception.
- **No audit trail:** When a secret changes, there is no history of who changed it, what it was before, or why.
- **Environment drift:** Dev, staging, and production environments gradually diverge when variable changes are not propagated consistently.
- **Onboarding friction:** New developers must request `.env` files from teammates, creating delays and human bottlenecks.
- **No access control:** Anyone with a shared file has access to all secrets, with no granularity or revocation capability.
- **Version control risk:** Secrets accidentally committed to git repositories expose sensitive credentials publicly.
- **Multi-project complexity:** Teams managing several services have no unified interface to view or compare environment configurations.

### 2.3 Impact

These issues result in security vulnerabilities, compliance failures, production incidents from configuration mismatches, and wasted engineering time. According to industry research, leaked credentials are among the top causes of cloud security incidents, and the median time to detect such leaks exceeds 80 days.

---

## 3. Product Overview

### 3.1 Product Description

EnvHub is a self-hosted, open-source secrets and environment variable management platform. It allows teams to organize variables by project, service, and environment (e.g., dev, staging, production), then push and pull those variables via a CLI tool or manage them through a web dashboard.

### 3.2 Key Differentiators

- **Zero-knowledge architecture:** All secrets live exclusively in the team's own Azure Key Vault. Neither the EnvHub creators nor any third party has access.
- **Git-inspired workflow:** Push/pull/history commands mirror familiar developer workflows, minimizing the learning curve.
- **Azure-native hosting:** React.js SPA and .NET 10 Azure Functions API deployed as a single unit via Azure Static Web Apps — one CI/CD pipeline, one deployment, globally distributed CDN out of the box.
- **Enterprise-native authentication:** Microsoft Entra ID (OIDC) SSO with MFA and Conditional Access support — no additional identity system to manage.
- **Azure-native storage:** Azure Key Vault handles secrets with enterprise-grade AES-256 encryption, FIPS 140-2 HSMs, versioning, and soft-delete.

### 3.3 Architecture Summary

| Layer | Technology | Role |
|---|---|---|
| Frontend | React.js + TypeScript | Single-page application (SPA) dashboard for browsing and managing secrets |
| Backend API | Azure Functions (.NET 10, Isolated Worker) | Serverless HTTP-triggered functions handling authentication, authorization, and Key Vault operations |
| Hosting | Azure Static Web Apps | Unified host for both the React.js SPA (CDN-distributed) and the linked Azure Functions API |
| Storage | Azure Key Vault | Secrets stored as Key Vault Secrets with versioning, HSM protection, and access policies |
| Authentication | Microsoft Entra ID (OIDC / OAuth 2.0) | Corporate SSO — user identity, tenant membership, and group-based authorization |
| Encryption | Azure Key Vault (AES-256, FIPS 140-2 Level 2 HSM) | Secrets encrypted at rest and in transit; keys managed by Azure |
| CLI | .NET 10 global tool (`dotnet tool install -g envhub`) | Cross-platform terminal tool for push/pull/history workflows (Windows, macOS, Linux) |

---

## 4. Target Users

### 4.1 Primary Personas

#### Persona 1: Backend / Full-Stack Developer
- Works on multiple services across dev, staging, and production.
- Frequently pulls environment configs to run local development.
- Needs a fast, CLI-native workflow that integrates with existing scripts.
- Values consistency: wants to know they have the latest secrets without asking teammates.

#### Persona 2: DevOps / Platform Engineer
- Responsible for provisioning environment configs across environments.
- Needs visibility into what secrets exist, who changed them, and when.
- Manages access for a growing team and needs organization-level controls.
- Requires audit trails for compliance (SOC2, ISO 27001, etc.).

#### Persona 3: Engineering Manager / Team Lead
- Onboards new developers and wants self-service secret access.
- Needs to ensure no secrets are shared over insecure channels.
- Wants a single source of truth for environment configuration.

#### Persona 4: Open-Source / Solo Developer
- Manages personal projects with multiple environments.
- Self-hosts on Vercel to maintain full data ownership.
- Wants a free, low-maintenance secrets store with no vendor lock-in.

### 4.2 Out of Scope Users

- Non-technical users who do not interact with environment configurations.
- Enterprise teams requiring on-premise deployment (future roadmap item).

---

## 5. Goals & Success Metrics

### 5.1 Product Goals

1. Eliminate insecure secret sharing practices within development teams.
2. Provide a complete, versioned audit trail for all environment variable changes.
3. Enable zero-friction onboarding for new team members via self-service secret access.
4. Maintain a zero-knowledge architecture so the platform operator never has access to user data.
5. Deliver a developer-first CLI experience that integrates seamlessly into existing workflows.

### 5.2 Key Performance Indicators (KPIs)

| Metric | Target | Timeframe |
|---|---|---|
| CLI adoption (active users) | 500+ active CLI users | 6 months post-launch |
| GitHub stars | 1,000+ | 6 months post-launch |
| Mean time to onboard (new dev) | < 5 minutes | At launch |
| Secret retrieval latency (p95) | < 500 ms | At launch |
| Zero unencrypted storage incidents | 100% | Ongoing |
| Version history accuracy | 100% attributable changes | At launch |
| Self-deployment success rate | > 90% on first attempt | At launch |

---

## 6. Functional Requirements

### 6.1 Authentication & Access Control

#### FR-AUTH-01: Microsoft Entra ID (OIDC) as Sole Identity Provider
- The system must use **Microsoft Entra ID** exclusively as the identity provider via OpenID Connect (OIDC) / OAuth 2.0.
- No other identity providers (GitHub, GitLab, Bitbucket, username/password) are supported.
- Authentication must follow the **OAuth 2.0 Authorization Code flow with PKCE** using the Microsoft identity platform endpoint: `https://login.microsoftonline.com/{tenant}/v2.0`.
- Required configuration variables:
  - `ENTRA_CLIENT_ID` — Azure App Registration client ID
  - `ENTRA_CLIENT_SECRET` — Azure App Registration client secret
  - `ENTRA_TENANT_ID` — Azure AD tenant ID
  - `ENTRA_ALLOWED_GROUPS` — *(Optional)* Comma-separated list of Entra ID security group object IDs; if omitted, all tenant members are granted access
- The Azure App Registration must request the following Microsoft Graph scopes: `openid`, `profile`, `email`, `User.Read`, `GroupMember.Read.All`.
- Upon successful login, the user's Entra ID **display name**, **UPN** (email), and **object ID** must be recorded for audit attribution on all secret operations.

#### FR-AUTH-02: Tenant & Group Gating
- Access is restricted to members of the configured Azure AD tenant (`ENTRA_TENANT_ID`).
- When `ENTRA_ALLOWED_GROUPS` is set, access is further restricted to members of the listed security groups or Microsoft 365 groups.
- Users who authenticate successfully but fail group membership checks must receive a clear "Access Denied" response and must not be issued a session token.
- Group membership must be re-verified on each session renewal, not only at initial login.

#### FR-AUTH-03: Microsoft Entra ID Integration in Azure Functions Backend
- Each Azure Function must validate the incoming `Authorization: Bearer <token>` header using `Microsoft.Identity.Web` or a lightweight JWT validation middleware compatible with the .NET 10 Isolated Worker model.
- The React.js frontend must use **MSAL.js** (`@azure/msal-react`) to handle the OIDC login flow and attach bearer tokens to all API requests.
- The Azure Static Web Apps built-in authentication (`/.auth/login/aad`) may optionally be used to complement MSAL.js for server-side session management.
- Every Function must validate the `aud`, `iss`, `tid`, and `scp` claims on incoming tokens.
- Token validation must use Microsoft's OIDC discovery document for key rotation resilience.

#### FR-AUTH-04: Multi-Factor Authentication & Conditional Access
- The platform must respect Entra ID Conditional Access policies configured by the organization (MFA requirements, device compliance, location restrictions).
- EnvHub must not attempt to bypass or work around MFA challenges — users must complete them in the Microsoft login flow before a session is granted.

#### FR-AUTH-05: Session Management
- The React.js frontend must use MSAL.js token caching (in-memory or sessionStorage) to maintain authenticated sessions.
- Access tokens must be refreshed silently using MSAL refresh token flows before expiry.
- The Azure Functions API must enforce token expiry — expired tokens must receive a `401 Unauthorized` response.
- On session expiry, the frontend must redirect users to the Entra ID login page automatically.

---

### 6.2 Hosting & Deployment Model

#### FR-HOST-01: Azure Static Web Apps as Unified Host
- The React.js SPA and the Azure Functions API must be deployed as a single unit to **Azure Static Web Apps**.
- The Azure Functions project must be configured as the **linked backend** of the Static Web App, exposed under the `/api/` route prefix automatically.
- The Static Web Apps CDN must serve the React.js frontend globally from Azure's edge network with no additional CDN configuration.

#### FR-HOST-02: Azure Functions API
- All backend logic must be implemented as **HTTP-triggered Azure Functions** using the **.NET 10 Isolated Worker** process model.
- Functions must be organized by domain: `secrets/`, `projects/`, `history/`, and `auth/`.
- Each Function must be stateless; no in-process state may be retained between invocations.
- The Functions app must use the `Azure.Security.KeyVault.Secrets` SDK to interact with Azure Key Vault.
- Local development must be supported via **Azure Functions Core Tools** (`func start`) alongside the React dev server.

#### FR-HOST-03: Routing & API Proxy
- Azure Static Web Apps must handle SPA client-side routing via `staticwebapp.config.json` (fallback to `index.html` for all non-API routes).
- CORS configuration must restrict API access to the Static Web Apps origin only in production.
- API routes must follow the convention `/api/{resource}/{id?}` (e.g., `/api/projects`, `/api/secrets/{project}/{service}/{env}`).

#### FR-HOST-04: Environment Configuration
- All secrets and configuration values for the Functions app must be stored in **Azure Static Web Apps application settings** (propagated to Azure Functions at runtime), never in source code.
- Local development must use `local.settings.json` (git-ignored) for Function app configuration.

### 6.4 Project Management

#### FR-PROJ-01: Create and Name Projects
- Users can create named projects (e.g., `e-commerce-platform`, `data-pipeline`).
- Project names must be unique within the deployment.
- Projects serve as the top-level organizational unit.

#### FR-PROJ-02: Service / Component Organization
- Within each project, users can create named services (e.g., `backend`, `frontend`, `worker`).
- Services allow grouping variables by application component.
- A project can contain multiple services.

#### FR-PROJ-03: Environment Support
- Each project/service combination supports multiple environments (e.g., `dev`, `staging`, `production`, `test`).
- Environments are user-defined strings — the platform does not restrict naming.
- Variables are stored per `project + service + environment` tuple.

---

### 6.5 Secret Management

#### FR-SEC-01: Create / Update Variables
- Users can add, edit, and remove key-value pairs in any project/service/environment.
- Variable keys must follow environment variable naming conventions (`UPPER_SNAKE_CASE` recommended).
- Each variable is stored as an individual **Azure Key Vault Secret**, named using the convention `{project}--{service}--{environment}--{KEY}`.
- Values can be arbitrary strings up to 25 KB (Azure Key Vault secret value limit).

#### FR-SEC-02: Azure Key Vault Secret Versioning
- Azure Key Vault natively versions every secret update. Each `SET` operation on a secret creates a new immutable version.
- EnvHub must leverage Key Vault's native versioning as the source of truth for version history, rather than maintaining a separate version store.
- Disabled secret versions must remain readable by administrators for audit purposes.

#### FR-SEC-03: Key Vault Access Policies
- EnvHub's serverless backend must authenticate to Azure Key Vault using a **Managed Identity** or a **Service Principal** with the following Key Vault permissions:
  - Secrets: `Get`, `List`, `Set`, `Delete`, `Recover`, `Backup`, `Restore`
  - (Optional) Keys: `Get`, `List` (if using Key Vault-managed encryption keys)
- End users must never have direct Azure Key Vault access; all operations must be proxied through EnvHub's API layer.

#### FR-SEC-04: Bulk Import from .env Files
- The CLI must support reading a local `.env` file and pushing all key-value pairs in a single command.
- Each key-value pair is written as a separate Key Vault secret in a single batched operation.
- The dashboard may optionally support file upload for bulk import.

#### FR-SEC-05: Export / Pull to .env Files
- The CLI `pull` command must support writing retrieved variables to a local `.env` file.
- Output must also be available as console-printed text to support tool piping.
- The API layer retrieves and returns secret values; secrets are never cached on the server.

#### FR-SEC-06: Change Documentation
- When pushing variables, users must optionally (or mandatorily) provide a reason/message for the change.
- This message is stored as an **Azure Key Vault Secret tag** (`change-message`, `changed-by`, `changed-at`) on each new secret version.

---

### 6.6 Version History & Audit

#### FR-VER-01: Full Version History
- Every push operation creates a new, immutable version in Azure Key Vault (native Key Vault versioning).
- The `history` command and dashboard must enumerate all Key Vault secret versions for a given project/service/environment.
- Each version record must include: Key Vault version ID, timestamp, author (GitHub username or Entra ID UPN), and change message (read from Key Vault secret tags).

#### FR-VER-02: Version Comparison *(Future)*
- Users should be able to compare two versions side-by-side to see which keys were added, modified, or removed.
- This feature is on the roadmap for v2.

#### FR-VER-03: Version Rollback *(Future)*
- Users should be able to restore a previous version as the active version.
- Rollback should create a new version entry (not overwrite history).

---

### 6.7 CLI Tool

#### FR-CLI-01: Initialize CLI
```
envhub init
```
- Prompts user to provide the deployment URL and authentication token.
- Stores configuration in a local config file (`~/.envhub/config`).

#### FR-CLI-02: Push Command
```
envhub push -p <project> -s <service> -e <env> [-r "Reason"] [--file .env]
```
- Reads local environment file or stdin.
- Encrypts all values before upload.
- Returns success/error status with a clear summary.

#### FR-CLI-03: Pull Command
```
envhub pull -p <project> -s <service> -e <env> [-o output.env]
```
- Fetches and decrypts variables from EnvHub.
- Writes to a specified file or prints to stdout.
- Supports piping to other tools.

#### FR-CLI-04: History Command
```
envhub history -p <project> -s <service> -e <env>
```
- Displays paginated version history.
- Shows timestamp, author, and change message for each version.

#### FR-CLI-05: Cross-Platform Support & Distribution
- The CLI must function on Windows, macOS, and Linux.
- The CLI is implemented in **.NET 10** as a cross-platform console application.
- Distribution must be available as a **global .NET tool** via NuGet.org: `dotnet tool install -g envhub`.
- Self-contained single-file executables must be published for each platform (Windows x64, macOS arm64/x64, Linux x64) for users without the .NET 10 runtime installed.
- The CLI must use `Azure.Identity` and `Microsoft.Identity.Client` (MSAL.NET) for Entra ID device code flow authentication during `envhub init`.

---

### 6.8 Web Dashboard

#### FR-DASH-01: Project Browser
- Users can see all projects they have access to on the dashboard home.
- Clicking a project shows its services and environments.

#### FR-DASH-02: Variable Viewer
- Users can browse the list of variable keys for any project/service/environment.
- Variable values must be masked by default (show/hide toggle).
- Users can edit values directly from the dashboard UI.

#### FR-DASH-03: Version History View
- The dashboard must show the full version history for any project/service/environment.
- History entries display timestamp, author avatar, and change message.

#### FR-DASH-04: Search & Filter
- Users can search for a variable key across projects and environments.
- Filter by project, service, environment, and date range.

---

## 7. Non-Functional Requirements

### 7.1 Security

| ID | Requirement | Priority |
|---|---|---|
| NFR-SEC-01 | All secret values must be stored in Azure Key Vault using AES-256 encryption backed by FIPS 140-2 Level 2 HSMs. | Critical |
| NFR-SEC-02 | The Azure Service Principal credentials (`AZURE_CLIENT_SECRET`) must never appear in source code, logs, or client responses. | Critical |
| NFR-SEC-03 | All API communication — including calls to Azure Key Vault — must use HTTPS/TLS 1.2+. | Critical |
| NFR-SEC-04 | Authentication tokens must not be stored in browser localStorage. | High |
| NFR-SEC-05 | Azure Key Vault diagnostic logging must be enabled; all secret Get/Set/Delete operations must be recorded with caller identity. | High |
| NFR-SEC-06 | Key Vault diagnostic logs must be streamable to Azure Monitor, Azure Sentinel, or third-party SIEMs (Splunk). | Medium |
| NFR-SEC-07 | Secret values must never appear in EnvHub server logs, Vercel logs, or Azure Application Insights traces. | Critical |
| NFR-SEC-08 | The Azure Key Vault must have soft-delete and purge protection enabled to prevent accidental or malicious secret deletion. | High |

### 7.2 Performance
- API response time for secret retrieval must be under 500 ms at p95, excluding Azure Function cold-start.
- The web dashboard must achieve a Lighthouse performance score of ≥ 80; the Static Web Apps CDN must serve static assets with appropriate `Cache-Control` headers.
- The CLI push/pull operations must complete within 3 seconds for payloads up to 100 variables.
- Azure Function cold-start time must not exceed 3 seconds; the `.NET 10 Isolated Worker` model with ahead-of-time (AOT) compilation must be evaluated to minimise cold starts.

### 7.3 Reliability & Availability
- Azure Static Web Apps provides a 99.95% SLA for the globally distributed SPA and a 99.95% SLA for the linked Azure Functions backend.
- Azure Key Vault provides a 99.99% availability SLA; each Azure Function must implement exponential backoff retry logic for transient Key Vault errors using the `Azure.Core` retry pipeline.
- The Azure Functions app must expose a dedicated health-check Function at `/api/health` returning HTTP 200 for monitoring and availability checks.
- The CLI must provide clear error messages and exponential backoff retry guidance on transient network failures.

### 7.4 Scalability
- The serverless architecture must scale to handle burst traffic without manual intervention.
- Azure Key Vault supports up to 25,000 secrets per vault and up to 2,000 transactions per 10 seconds (Standard SKU); multiple vaults can be provisioned per region if limits are approached.
- The platform must support teams with up to 1,000 projects and 10,000 variables without degradation.
- For organizations requiring data residency in specific regions, separate Key Vault instances must be provisionable per region.

### 7.5 Usability
- A new user must be able to complete their first push within 5 minutes of deployment.
- CLI help text must be comprehensive and include examples for all commands.
- Dashboard UI must be accessible on standard desktop browsers (Chrome, Firefox, Safari, Edge).
- UI must be responsive for tablet-sized screens (min. 768px width).

### 7.6 Maintainability
- The codebase must be MIT-licensed and open-source on GitHub.
- CI/CD must be implemented via **Azure Static Web Apps' built-in GitHub Actions workflow** (auto-generated on resource creation), which builds and deploys both the React.js SPA and the Azure Functions app in a single pipeline.
- The Azure Functions backend (.NET 10) must have at least 80% unit test coverage using **xUnit** and **Moq**; Functions must be testable in isolation without requiring a live Azure environment.
- The React.js frontend must use TypeScript strict mode and have component-level tests using **Jest + React Testing Library**.
- The .NET 10 CLI must have at least 80% unit test coverage using **xUnit** and **Moq**.
- The Azure Functions app must expose an HTTP Function at `/api/swagger` serving an **OpenAPI 3.0** specification for documentation and integration testing using `Microsoft.Azure.Functions.Worker.Extensions.OpenApi`.

---

## 8. User Stories

| ID | As a… | I want to… | So that… | Priority |
|---|---|---|---|---|
| US-01 | Developer | Log in with my Microsoft corporate account via SSO | I can access team secrets without a separate account | Must Have |
| US-02 | Developer | Pull the latest .env for a service | I can run the app locally with current configs | Must Have |
| US-03 | DevOps Eng. | Push updated variables for production | The team has the latest secrets without file sharing | Must Have |
| US-04 | DevOps Eng. | View the history of changes to a service | I can audit who changed what and when | Must Have |
| US-05 | IT Admin | Restrict access via Entra ID security groups | Only members of specific Azure AD groups can reach secrets | Must Have |
| US-06 | Developer | Add a reason when pushing changes | Future reviewers understand why a change was made | Should Have |
| US-07 | Developer | Search for a variable key across projects | I can find where a variable is used quickly | Should Have |
| US-08 | DevOps Eng. | Export audit logs to my SIEM tool | We maintain compliance records for SOC2 audits | Should Have |
| US-09 | Developer | Compare two versions of my .env | I can see what changed between deploys | Could Have |
| US-10 | Team Lead | Roll back to a previous version | I can quickly recover from a bad configuration push | Could Have |
| US-11 | DevOps Eng. | Use AWS S3 as the storage backend | Data remains within our AWS infrastructure | Won't Have (v1) |
| US-12 | Manager | Assign role-based access (read vs. write) | Junior devs can read but not modify production secrets | Won't Have (v1) |

---

## 9. Deployment & Self-Hosting

### 9.1 Prerequisites
- An **Azure subscription** with permissions to create a Static Web App, Key Vault, and App Registration.
- **.NET 10 SDK** and **Azure Functions Core Tools v4** for local development and testing of the Functions backend.
- **Node.js 20+** for building the React.js frontend.
- **.NET 10 runtime** on developer machines for CLI usage (or use the self-contained binary — no runtime required).
- A GitHub repository (required for Azure Static Web Apps' built-in CI/CD integration).

### 9.2 Azure Setup

**Step 1 — Register an App Registration for user authentication:**
```bash
az ad app create --display-name "EnvHub-Auth" \
  --sign-in-audience AzureADMyOrg \
  --spa-redirect-uris "https://<your-static-web-app>.azurestaticapps.net/auth/callback"
```
Grant the following Microsoft Graph **Delegated** permissions: `openid`, `profile`, `email`, `User.Read`, `GroupMember.Read.All`.

**Step 2 — Provision Azure Key Vault:**
```bash
az keyvault create --name <vault-name> --resource-group <rg> --location <region> \
  --sku standard --enable-soft-delete true --enable-purge-protection true
```

**Step 3 — Provision Azure Static Web Apps and link the Functions backend:**
```bash
az staticwebapp create --name envhub --resource-group <rg> \
  --source https://github.com/<org>/<repo> --branch main \
  --app-location "/frontend" --api-location "/api" --output-location "build"
```
Azure Static Web Apps automatically creates a GitHub Actions workflow that builds and deploys both the React.js SPA and the .NET 10 Azure Functions API on every push to `main`.

**Step 4 — Assign a Managed Identity to the Static Web App and grant Key Vault access:**
```bash
# Enable system-assigned managed identity on the Static Web App
az staticwebapp identity assign --name envhub --resource-group <rg>

# Grant the managed identity Key Vault Secrets Officer role
az keyvault set-policy --name <vault-name> \
  --object-id <managed-identity-object-id> \
  --secret-permissions get list set delete recover backup restore
```
Using a **Managed Identity** (instead of a Service Principal with a client secret) eliminates the need to store Key Vault credentials in application settings.

### 9.3 Application Settings

Configure the following in the Azure Static Web Apps **Application Settings** (propagated automatically to the linked Azure Functions):

| Setting | Description |
|---|---|
| `ENTRA_CLIENT_ID` | App Registration client ID for user authentication |
| `ENTRA_CLIENT_SECRET` | App Registration client secret |
| `ENTRA_TENANT_ID` | Azure AD tenant ID |
| `ENTRA_ALLOWED_GROUPS` | *(Optional)* Comma-separated Entra ID group object IDs |
| `AZURE_VAULT_URL` | Key Vault URL (e.g., `https://<vault-name>.vault.azure.net/`) |

When using a **Managed Identity** for Key Vault access, `AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET`, and `AZURE_TENANT_ID` are not required — the Functions runtime authenticates automatically.

### 9.4 Local Development

```bash
# Terminal 1 — Start the Azure Functions API locally
cd /api && func start

# Terminal 2 — Start the React.js SPA (proxies /api/* to localhost:7071)
cd /frontend && npm start
```
Local settings are stored in `/api/local.settings.json` (git-ignored). The React dev server is pre-configured in `package.json` to proxy `/api/*` requests to the local Functions host.

**CLI:**
Initialize the CLI: `envhub init` (follow prompts to provide the Static Web App URL and authenticate via Entra ID).

### 9.5 Zero-Knowledge Guarantee

> **Security Principle:** When you self-host EnvHub using your own Azure subscription, all secrets live exclusively in your Azure Key Vault. The Azure Functions API and React.js SPA run entirely within your Azure Static Web Apps instance. Managed Identity eliminates any shared credentials. Neither the EnvHub creators nor any third party has access to your plaintext secrets — full data sovereignty is maintained within your Azure tenant.

---

## 10. Security Model

### 10.1 Encryption Architecture

EnvHub delegates all secrets storage and encryption to **Azure Key Vault**, which provides AES-256 encryption at rest and TLS encryption in transit, backed by FIPS 140-2 Level 2 validated HSMs (or Level 3 with Premium SKU). There is no application-level encryption key to manage — Azure owns the key hierarchy within your tenant. The EnvHub backend authenticates to Key Vault using a Service Principal with least-privilege secret permissions, and secret values are never cached, logged, or persisted outside of Key Vault.

### 10.2 Authentication Flow (Microsoft Entra ID OIDC)

1. The React.js frontend initializes **MSAL.js** with `ENTRA_CLIENT_ID` and `ENTRA_TENANT_ID` on app load.
2. Unauthenticated users are redirected to the Microsoft identity platform login page (with PKCE challenge).
3. The user authenticates with their corporate Microsoft credentials. Conditional Access policies (MFA, device compliance) are enforced here by Azure.
4. Microsoft redirects back to the React.js app with an authorization code.
5. MSAL.js exchanges the code for an **ID token** and **access token**; tokens are cached in-memory.
6. MSAL.js attaches the bearer access token to every HTTP request sent to the `/api/*` routes served by Azure Static Web Apps and forwarded to the linked Azure Functions backend.
7. The Azure Function validates the token using JWT middleware (.NET 10 Isolated Worker): verifying signature, `aud`, `iss`, `tid`, and `exp` claims against Microsoft's OIDC discovery document.
8. If `ENTRA_ALLOWED_GROUPS` is configured, the Function calls **Microsoft Graph** (`GET /v1.0/me/memberOf`) to verify group membership. Unauthorized users receive `403 Forbidden`.
9. On success, the API processes the request; the user's UPN and display name from the token claims are used for audit tagging on Key Vault secret operations.
10. MSAL.js silently refreshes the access token before expiry using the cached refresh token. Expired sessions trigger a re-login redirect.

### 10.3 Threat Model

| Threat | Mitigation |
|---|---|
| Unauthorized access to secrets | Entra ID OIDC with `ENTRA_ALLOWED_GROUPS` gating and ASP.NET Core token validation ensures only authorized corporate identities can access the API |
| Secrets exposed via storage breach | Secrets live only in Azure Key Vault (AES-256, FIPS 140-2 HSM); no plaintext ever leaves Key Vault except over authenticated TLS calls |
| Secrets leaked in logs | Secret values are never written to server, Vercel, or Azure Monitor logs; only secret names and metadata are logged |
| Man-in-the-middle attacks | All traffic encrypted via HTTPS/TLS 1.2+; Key Vault enforces TLS on all API calls |
| Insider threats | Azure Key Vault diagnostic logs capture every Get/Set/Delete operation with caller identity; logs exportable to Azure Monitor, Splunk, or Azure Sentinel |
| Key compromise | Key Vault encryption keys are managed by Azure; customers can optionally bring their own key (BYOK) for full control |
| Unauthorized Key Vault access | Service Principal follows least-privilege; no interactive user has direct Key Vault access; all access is proxied through EnvHub's API |
| Accidental credential exposure in .env push | CLI validates file format before upload; Key Vault enforces secret name character restrictions |

---

## 11. Product Roadmap

### Version 1.0 — Current (MVP)
- **Microsoft Entra ID (OIDC)** as the sole identity provider, with tenant and security-group gating.
- **React.js + TypeScript** SPA frontend with MSAL.js integration, hosted on Azure Static Web Apps.
- **.NET 10 Azure Functions** (Isolated Worker) REST API, deployed as the linked backend of the Static Web App.
- Single CI/CD pipeline via Azure Static Web Apps' auto-generated GitHub Actions workflow.
- **Managed Identity** for Key Vault access — no stored credentials.
- Project / service / environment variable management.
- Full version history via Azure Key Vault native secret versioning with author attribution via secret tags.
- **.NET 10 CLI tool** (`dotnet tool install -g envhub`) for push, pull, and history commands (cross-platform: Windows, macOS, Linux).
- **Azure Key Vault** as the secrets storage backend (AES-256, FIPS 140-2 HSM, soft-delete + purge protection).

### Version 1.x — Short-term
- Duplicate-content detection: skip version creation when push content is identical to the current Key Vault secret version.
- Version comparison: side-by-side diff view of two Key Vault secret version snapshots.
- Dashboard variable search and cross-project key lookup.
- CLI version pinning: pull a specific Key Vault secret version with `--version <version-id>` flag.
- Publish CLI to NuGet.org as a global .NET tool: `dotnet tool install -g envhub`.
- Azure Key Vault diagnostic log streaming to Azure Sentinel and Splunk via Azure Monitor.
- Bring Your Own Key (BYOK): support for customer-managed keys in Azure Key Vault.
- Azure Static Web Apps staging environments: preview deployments on pull requests.

### Version 2.0 — Medium-term
- Role-based access control (RBAC): reader, writer, admin roles per project, enforced via Entra ID groups and Key Vault access policies.
- Version rollback from dashboard and CLI (re-enable a prior Key Vault secret version).
- Multi-vault support: route different projects to different Azure Key Vault instances (e.g., per region or compliance boundary).
- Webhook notifications on secret changes via Azure Event Grid integration.
- CI/CD native integrations (Azure DevOps Pipelines, GitHub Actions).
- Bring Your Own Functions: support swapping Azure Static Web Apps' linked backend for a dedicated Azure Functions Premium Plan (for VNet integration and no cold starts).

### Future Considerations
- Enterprise on-premise deployment guide (Docker/Kubernetes).
- Secret rotation automation for common providers (AWS, Azure, Stripe, etc.).
- Secret sharing with expiring links (no-account access for contractors).
- Mobile-responsive dashboard improvements.
- Terraform provider for infrastructure-as-code workflows.

---

## 12. Competitive Landscape

The environment variable and secrets management market includes several well-established players. EnvHub differentiates primarily through its zero-knowledge, self-hosted model and developer-first Git-inspired workflow.

| Feature | EnvHub | HashiCorp Vault | AWS Secrets Manager | Doppler | .env files |
|---|---|---|---|---|---|
| Self-hosted / zero-knowledge | ✅ Yes | ✅ Yes | ❌ No | ❌ No | ✅ Yes |
| Git-inspired CLI workflow | ✅ Yes | ❌ No | ❌ No | ⚠️ Partial | ❌ No |
| Version history | ✅ Yes | ✅ Yes | ✅ Yes | ✅ Yes | ❌ No |
| Free tier / open-source | ✅ MIT | ⚠️ OSS core | ❌ Pay per use | ❌ Paid | ✅ Free |
| No infrastructure required | ⚠️ Azure Static Web Apps + Key Vault | ❌ Server needed | ⚠️ Managed | ✅ SaaS | ✅ None |
| Encryption at rest | ✅ AES-256 HSM (Key Vault) | ✅ AES-256 | ✅ AES-256 | ✅ Yes | ❌ Plaintext |
| Team / org access control | ✅ GitHub Org + Entra ID | ✅ LDAP/OIDC | ✅ IAM | ✅ Roles | ❌ No |
| Audit logging | ✅ Yes | ✅ Yes | ✅ Yes | ✅ Yes | ❌ No |

---

## 13. Constraints & Assumptions

### 13.1 Constraints
- Microsoft Entra ID is the only supported identity provider. Other providers (GitHub, GitLab, Okta, Auth0) are out of scope.
- Azure Key Vault is the only supported storage backend in v1. AWS Secrets Manager and HashiCorp Vault are planned for v2.
- Azure Key Vault Standard SKU supports up to 2,000 secret transactions per 10 seconds; high-volume teams may need to provision multiple vaults.
- Azure Static Web Apps' linked Functions backend uses the **Consumption plan** by default; cold starts may occur. A Functions Premium Plan can be linked for production workloads requiring consistent latency.
- The CLI requires **.NET 10 runtime** (or use the self-contained binary for runtimless installation).
- The platform does not currently support secret rotation automation (planned for v2 via Azure Key Vault rotation policies).
- RBAC is not available in v1; access is binary (in the Entra ID tenant/group or not).

### 13.2 Assumptions
- All team members have Microsoft 365 / Azure AD accounts within the configured Entra ID tenant.
- The deployment administrator has permissions to register an Azure App Registration, create a Static Web App and Key Vault, and assign Managed Identity roles.
- The repository is hosted on GitHub (required for Azure Static Web Apps CI/CD integration).
- Teams are comfortable provisioning Azure resources and managing Entra ID app registrations.
- Teams accept Azure Key Vault as the storage backend for v1 deployments.
- End users who install the CLI have .NET 10 runtime installed, or use the self-contained binary.
- Internet access is available for all Azure-hosted services and end-user machines.

### 13.3 Out of Scope (v1)
- On-premise or air-gapped deployments.
- Identity providers other than Microsoft Entra ID (e.g., GitHub, GitLab, Okta, Auth0).
- Alternative storage backends (AWS Secrets Manager, HashiCorp Vault) — planned for v2.
- Secret rotation, expiration policies, or automated secret lifecycle management (available in v2 via Key Vault rotation policies).
- Mobile app (iOS/Android).
- Real-time collaboration (e.g., live variable editing with conflict resolution).

---

## 14. Glossary

| Term | Definition |
|---|---|
| Environment Variable | A key-value pair used to configure application behavior at runtime. |
| .env File | A plain text file containing environment variables in `KEY=VALUE` format. |
| Secret | Any sensitive environment variable value such as API keys, tokens, or passwords. |
| Azure Functions | Microsoft Azure's serverless compute service for running event-driven code without managing infrastructure; used here as the EnvHub API backend in Isolated Worker (.NET 10) mode. |
| Azure Static Web Apps | A cloud hosting service that serves static web apps (React.js SPA) from a global CDN with a natively linked Azure Functions backend under the `/api/` route. |
| Isolated Worker Model | The .NET Azure Functions execution model where Functions run in a separate process from the Functions runtime, enabling full .NET 10 support. |
| Managed Identity | An Azure feature that provides an automatically managed identity for Azure resources to authenticate to other Azure services (e.g., Key Vault) without storing credentials. |
| MSAL.NET | Microsoft Authentication Library for .NET — used in the .NET 10 CLI to handle Entra ID device code flow authentication. |
| MSAL.js | Microsoft Authentication Library for JavaScript — used in the React.js frontend to handle Entra ID login and token management. |
| Microsoft.Identity.Web | A .NET library that simplifies integrating Microsoft Entra ID token validation into Azure Functions and ASP.NET Core applications. |
| OpenAPI / Swagger | A standard specification for REST API documentation; the Azure Functions backend exposes this at `/api/swagger`. |
| dotnet tool | A NuGet-based mechanism for distributing .NET console applications as globally installable tools (`dotnet tool install -g envhub`). |
| Zero-Knowledge | An architecture where the service operator has no technical ability to access stored user data. |
| Serverless | A cloud execution model where the provider automatically manages infrastructure scaling. |
| Azure Key Vault | Microsoft Azure's cloud service for securely storing and accessing secrets, keys, and certificates, backed by FIPS 140-2 HSMs. |
| Service Principal | An Azure identity used by applications to authenticate to Azure services (such as Key Vault) without user interaction. |
| HSM | Hardware Security Module — a physical device that provides tamper-resistant cryptographic key storage. |
| BYOK | Bring Your Own Key — an Azure Key Vault feature allowing customers to import and manage their own encryption keys. |
| Soft-Delete | An Azure Key Vault feature that retains deleted secrets for a configurable retention period before permanent removal. |
| Purge Protection | An Azure Key Vault feature that prevents secrets from being permanently deleted during the soft-delete retention period. |
| AZURE_VAULT_URL | The EnvHub config variable pointing to the Azure Key Vault endpoint (e.g., `https://<vault-name>.vault.azure.net/`). |
| ALLOWED_ORGS | An EnvHub config variable listing GitHub organizations whose members may access the deployment. |
| Microsoft Entra ID | Microsoft's cloud-based identity and access management service (formerly Azure Active Directory). |
| OIDC | OpenID Connect — an authentication protocol built on OAuth 2.0 used by Microsoft Entra ID. |
| ENTRA_TENANT_ID | The Azure AD tenant identifier used to scope authentication to a specific organization. |
| ENTRA_ALLOWED_GROUPS | Optional config listing Entra ID security group object IDs whose members may access the deployment. |
| UPN | User Principal Name — the Entra ID login identifier in the format `user@domain.com`. |
| PKCE | Proof Key for Code Exchange — a security extension to OAuth 2.0 that prevents authorization code interception. |
| Bearer Token | A JWT access token issued by Entra ID, attached to every API request as an `Authorization: Bearer <token>` header. |
| RBAC | Role-Based Access Control — a method of restricting access based on user roles. |
| SIEM | Security Information and Event Management — a system for collecting and analyzing security event logs. |
| SOC2 | Service Organization Control 2 — a compliance framework for SaaS security and confidentiality. |

---

## 15. Document History

| Version | Date | Author | Description |
|---|---|---|---|
| 1.0 | March 27, 2026 | Alberto Diaz | Initial PRD — created from application analysis of env-hub-nu.vercel.app |
| 1.1 | March 27, 2026 | Alberto Diaz | Added Microsoft Entra ID (OIDC) as a supported identity provider alongside GitHub OAuth |
| 1.2 | March 27, 2026 | Alberto Diaz | Replaced Vercel Blob with Azure Key Vault as the secrets storage backend; updated security model, deployment guide, NFRs, and roadmap accordingly |
| 1.3 | March 27, 2026 | Alberto Diaz | Replaced Next.js with React.js (frontend) + ASP.NET Core .NET 8 (backend); removed GitHub OAuth; Microsoft Entra ID is now the sole identity provider |
| 1.4 | March 27, 2026 | Alberto Diaz | Replaced ASP.NET Core Web API with Azure Functions (.NET 10, Isolated Worker); adopted Azure Static Web Apps as unified host for SPA + Functions; upgraded CLI from Python to .NET 10 global tool; added Managed Identity for Key Vault access |

---

*For questions or feedback on this document, contact: adiazcan@hotmail.com*
