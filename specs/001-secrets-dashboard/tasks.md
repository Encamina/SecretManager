---

description: "Task list for Secrets Management Dashboard implementation"
---

# Tasks: Secrets Management Dashboard

**Input**: Design documents from `/specs/001-secrets-dashboard/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Automated tests are included because the constitution and plan require an independently testable library-first implementation with backend and frontend validation.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `backend/src/`, `backend/tests/`
- **Frontend**: `frontend/src/`, `frontend/tests/`
- **Docs**: `specs/001-secrets-dashboard/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Initialize the repository structure for the backend API, backend domain library, infrastructure adapters, and React frontend.

- [x] T001 Create the backend and frontend solution structure in `backend/src/`, `backend/tests/`, `frontend/src/`, and `frontend/tests/`
- [x] T002 Initialize the Azure Functions isolated worker solution files in `backend/src/SecretManager.SecretsDashboard.Api/SecretManager.SecretsDashboard.Api.csproj`, `backend/src/SecretManager.SecretsDashboard.Domain/SecretManager.SecretsDashboard.Domain.csproj`, and `backend/src/SecretManager.SecretsDashboard.Infrastructure/SecretManager.SecretsDashboard.Infrastructure.csproj`
- [x] T003 [P] Initialize the React + TypeScript frontend project in `frontend/package.json`, `frontend/tsconfig.json`, and `frontend/src/main.tsx`
- [x] T004 [P] Configure baseline test projects and frontend test tooling in `backend/tests/SecretManager.SecretsDashboard.Domain.Tests/SecretManager.SecretsDashboard.Domain.Tests.csproj`, `backend/tests/SecretManager.SecretsDashboard.Api.Tests/SecretManager.SecretsDashboard.Api.Tests.csproj`, `frontend/jest.config.ts`, and `frontend/src/test/setupTests.ts`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build the shared infrastructure that every user story depends on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [x] T005 Define domain contracts for secret storage, metadata storage, access control, and audit recording in `backend/src/SecretManager.SecretsDashboard.Domain/Abstractions/ISecretStore.cs`, `backend/src/SecretManager.SecretsDashboard.Domain/Abstractions/IMetadataCatalog.cs`, `backend/src/SecretManager.SecretsDashboard.Domain/Abstractions/IAccessPolicy.cs`, and `backend/src/SecretManager.SecretsDashboard.Domain/Abstractions/IAuditEventRecorder.cs`
- [x] T006 [P] Create core domain models for access context and shared secret metadata in `backend/src/SecretManager.SecretsDashboard.Domain/Models/AccessContext.cs`, `backend/src/SecretManager.SecretsDashboard.Domain/Models/SecretIdentifier.cs`, and `backend/src/SecretManager.SecretsDashboard.Domain/Models/AuditEvent.cs`
- [x] T007 [P] Implement Azure Key Vault secret-store adapter in `backend/src/SecretManager.SecretsDashboard.Infrastructure/Secrets/KeyVaultSecretStore.cs`
- [x] T008 [P] Implement Azure Table Storage metadata adapters in `backend/src/SecretManager.SecretsDashboard.Infrastructure/Metadata/TableProjectCatalog.cs`, `backend/src/SecretManager.SecretsDashboard.Infrastructure/Metadata/TableServiceCatalog.cs`, and `backend/src/SecretManager.SecretsDashboard.Infrastructure/Metadata/TableEnvironmentCatalog.cs`
- [x] T009 [P] Configure Microsoft Entra ID token validation and role-based authorization in `backend/src/SecretManager.SecretsDashboard.Api/Program.cs`, `backend/src/SecretManager.SecretsDashboard.Api/Auth/EntraAuthorizationExtensions.cs`, and `backend/src/SecretManager.SecretsDashboard.Api/Auth/RequiredRoles.cs`
- [x] T010 [P] Configure Application Insights audit/event recording in `backend/src/SecretManager.SecretsDashboard.Infrastructure/Telemetry/ApplicationInsightsAuditEventRecorder.cs`
- [x] T011 Implement the domain orchestration service shell in `backend/src/SecretManager.SecretsDashboard.Domain/Services/SecretsDashboardService.cs`
- [x] T012 [P] Add shared frontend authentication and API client setup in `frontend/src/app/providers/AuthProvider.tsx`, `frontend/src/services/http/apiClient.ts`, and `frontend/src/services/auth/msalConfig.ts`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel.

---

## Phase 3: User Story 1 - Browse secrets by context (Priority: P1) 🎯 MVP

**Goal**: Let authenticated developers browse available projects, services, environments, and secret summaries by grouping.

**Independent Test**: Sign in through Microsoft Entra ID, navigate project -> service -> environment, and verify the correct secret summaries are shown for the selected grouping.

- [ ] T013 [P] [US1] Add backend domain tests for metadata browsing and grouping authorization in `backend/tests/SecretManager.SecretsDashboard.Domain.Tests/Services/SecretsDashboardBrowseTests.cs`
- [ ] T014 [P] [US1] Add backend API tests for catalog and secret-list endpoints in `backend/tests/SecretManager.SecretsDashboard.Api.Tests/Secrets/BrowseSecretsApiTests.cs`
- [ ] T015 [US1] Implement browse models for projects, services, environments, and secret summaries in `backend/src/SecretManager.SecretsDashboard.Domain/Models/ProjectSummary.cs`, `backend/src/SecretManager.SecretsDashboard.Domain/Models/ServiceSummary.cs`, `backend/src/SecretManager.SecretsDashboard.Domain/Models/EnvironmentSummary.cs`, and `backend/src/SecretManager.SecretsDashboard.Domain/Models/SecretSummary.cs`
- [ ] T016 [US1] Implement metadata browse and secret-list flows in `backend/src/SecretManager.SecretsDashboard.Domain/Services/SecretsDashboardService.cs`
- [ ] T017 [US1] Implement catalog and secret-list Azure Functions endpoints in `backend/src/SecretManager.SecretsDashboard.Api/Functions/GetCurrentUserFunction.cs`, `backend/src/SecretManager.SecretsDashboard.Api/Functions/ListProjectsFunction.cs`, `backend/src/SecretManager.SecretsDashboard.Api/Functions/ListServicesFunction.cs`, `backend/src/SecretManager.SecretsDashboard.Api/Functions/ListEnvironmentsFunction.cs`, and `backend/src/SecretManager.SecretsDashboard.Api/Functions/ListSecretsFunction.cs`
- [ ] T018 [P] [US1] Add frontend query hooks for user context and metadata navigation in `frontend/src/features/secrets-dashboard/api/getCurrentUser.ts`, `frontend/src/features/secrets-dashboard/api/listProjects.ts`, `frontend/src/features/secrets-dashboard/api/listServices.ts`, `frontend/src/features/secrets-dashboard/api/listEnvironments.ts`, and `frontend/src/features/secrets-dashboard/api/listSecrets.ts`
- [ ] T019 [US1] Build the browse experience in `frontend/src/features/secrets-dashboard/components/ProjectSelector.tsx`, `frontend/src/features/secrets-dashboard/components/ServiceSelector.tsx`, `frontend/src/features/secrets-dashboard/components/EnvironmentSelector.tsx`, `frontend/src/features/secrets-dashboard/components/SecretList.tsx`, and `frontend/src/features/secrets-dashboard/pages/SecretsDashboardPage.tsx`
- [ ] T020 [P] [US1] Add frontend tests for sign-in gating and grouped browsing in `frontend/tests/features/secrets-dashboard/SecretsDashboardBrowse.test.tsx`

**Checkpoint**: User Story 1 is independently functional and demonstrates the MVP browse workflow.

---

## Phase 4: User Story 2 - View secret details safely (Priority: P2)

**Goal**: Let authenticated and authorized developers view a selected environment variable and its metadata safely.

**Independent Test**: Open a secret from a selected grouping and verify the current value and metadata are shown only for authorized users.

- [ ] T021 [P] [US2] Add backend domain tests for authorized secret viewing and denied access handling in `backend/tests/SecretManager.SecretsDashboard.Domain.Tests/Services/SecretsDashboardViewTests.cs`
- [ ] T022 [P] [US2] Add backend API tests for the secret-detail endpoint in `backend/tests/SecretManager.SecretsDashboard.Api.Tests/Secrets/ViewSecretApiTests.cs`
- [ ] T023 [US2] Implement secret-detail retrieval and audit emission in `backend/src/SecretManager.SecretsDashboard.Domain/Models/SecretDetail.cs` and `backend/src/SecretManager.SecretsDashboard.Domain/Services/SecretsDashboardService.cs`
- [ ] T024 [US2] Implement the secret-detail Azure Function endpoint in `backend/src/SecretManager.SecretsDashboard.Api/Functions/GetSecretDetailFunction.cs`
- [ ] T025 [P] [US2] Add frontend secret-detail data access and presentation in `frontend/src/features/secrets-dashboard/api/getSecretDetail.ts`, `frontend/src/features/secrets-dashboard/components/SecretDetailPanel.tsx`, and `frontend/src/features/secrets-dashboard/components/SecretMetadataList.tsx`
- [ ] T026 [P] [US2] Add frontend tests for authorized and unauthorized secret viewing in `frontend/tests/features/secrets-dashboard/SecretDetailPanel.test.tsx`

**Checkpoint**: User Stories 1 and 2 both work independently, and viewing a secret produces safe audit evidence.

---

## Phase 5: User Story 3 - Edit environment variables (Priority: P3)

**Goal**: Let authorized developers update a secret value within the selected grouping and handle version conflicts explicitly.

**Independent Test**: Edit a secret with the latest version and confirm the update succeeds; repeat with a stale version and confirm a conflict is shown without silent overwrite.

- [ ] T027 [P] [US3] Add backend domain tests for successful edits, stale-version conflicts, and edit authorization in `backend/tests/SecretManager.SecretsDashboard.Domain.Tests/Services/SecretsDashboardEditTests.cs`
- [ ] T028 [P] [US3] Add backend API tests for the secret-update endpoint and 409 conflict responses in `backend/tests/SecretManager.SecretsDashboard.Api.Tests/Secrets/EditSecretApiTests.cs`
- [ ] T029 [US3] Implement secret-update request/response models and conflict handling in `backend/src/SecretManager.SecretsDashboard.Domain/Models/UpdateSecretRequest.cs`, `backend/src/SecretManager.SecretsDashboard.Domain/Models/UpdateSecretResult.cs`, and `backend/src/SecretManager.SecretsDashboard.Domain/Services/SecretsDashboardService.cs`
- [ ] T030 [US3] Implement the secret-update Azure Function endpoint in `backend/src/SecretManager.SecretsDashboard.Api/Functions/UpdateSecretFunction.cs`
- [ ] T031 [P] [US3] Build the edit form and conflict resolution UX in `frontend/src/features/secrets-dashboard/components/EditSecretForm.tsx`, `frontend/src/features/secrets-dashboard/components/SecretConflictBanner.tsx`, and `frontend/src/features/secrets-dashboard/api/updateSecret.ts`
- [ ] T032 [P] [US3] Add frontend tests for successful edits and stale-version conflicts in `frontend/tests/features/secrets-dashboard/EditSecretForm.test.tsx`
- [ ] T033 [US3] Wire the edit workflow into the dashboard page in `frontend/src/features/secrets-dashboard/pages/SecretsDashboardPage.tsx` and `frontend/src/features/secrets-dashboard/state/secretsDashboardReducer.ts`

**Checkpoint**: All user stories are independently functional, including explicit conflict handling for edits.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize documentation, operational hardening, and end-to-end validation across all stories.

- [ ] T034 [P] Update environment configuration and onboarding documentation in `README.md` and `specs/001-secrets-dashboard/quickstart.md`
- [ ] T035 Verify plaintext secret values are never logged or written to Azure Table Storage in `backend/tests/SecretManager.SecretsDashboard.Api.Tests/Telemetry/SecretSafetyTests.cs` and `backend/tests/SecretManager.SecretsDashboard.Domain.Tests/Metadata/MetadataSafetyTests.cs`
- [ ] T036 Review production-role authorization, Managed Identity permissions, and metadata-table access assumptions in `backend/src/SecretManager.SecretsDashboard.Api/Auth/RequiredRoles.cs` and `specs/001-secrets-dashboard/research.md`
- [ ] T037 Run the full validation flow described in `specs/001-secrets-dashboard/quickstart.md` and record any follow-up notes in `specs/001-secrets-dashboard/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories.
- **User Story 1 (Phase 3)**: Depends on Foundational completion.
- **User Story 2 (Phase 4)**: Depends on Foundational completion and can reuse the browse shell from User Story 1.
- **User Story 3 (Phase 5)**: Depends on Foundational completion and on User Story 2's detail flow for the latest secret version.
- **Polish (Phase 6)**: Depends on all desired user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: First delivery slice and recommended MVP.
- **User Story 2 (P2)**: Builds on the selected grouping from User Story 1 but remains independently testable once browse is available.
- **User Story 3 (P3)**: Builds on the selected grouping and detail retrieval from User Stories 1 and 2.

### Within Each User Story

- Tests for the story before or alongside implementation in the same phase.
- Domain models and services before HTTP endpoints.
- Backend contract integration before frontend wiring.
- Story-specific UI integration after the related backend capability is implemented.

### Parallel Opportunities

- Phase 1 setup tasks T003 and T004 can run in parallel after T001.
- In Phase 2, T007, T008, T009, T010, and T012 can run in parallel once T005 is defined.
- In User Story 1, T013, T014, T018, and T020 can run in parallel with coordinated mocks.
- In User Story 2, T021, T022, T025, and T026 can run in parallel once the story contract is agreed.
- In User Story 3, T027, T028, T031, and T032 can run in parallel once the update contract is stable.

---

## Parallel Example: User Story 1

```bash
# Parallel backend and frontend validation for grouped browsing
Task: "T013 [US1] backend domain tests in backend/tests/SecretManager.SecretsDashboard.Domain.Tests/Services/SecretsDashboardBrowseTests.cs"
Task: "T014 [US1] backend API tests in backend/tests/SecretManager.SecretsDashboard.Api.Tests/Secrets/BrowseSecretsApiTests.cs"
Task: "T018 [US1] frontend data hooks in frontend/src/features/secrets-dashboard/api/listProjects.ts"
Task: "T020 [US1] frontend tests in frontend/tests/features/secrets-dashboard/SecretsDashboardBrowse.test.tsx"
```

## Parallel Example: User Story 2

```bash
# Parallel detail-view delivery tasks
Task: "T021 [US2] domain tests in backend/tests/SecretManager.SecretsDashboard.Domain.Tests/Services/SecretsDashboardViewTests.cs"
Task: "T022 [US2] API tests in backend/tests/SecretManager.SecretsDashboard.Api.Tests/Secrets/ViewSecretApiTests.cs"
Task: "T025 [US2] frontend detail panel in frontend/src/features/secrets-dashboard/components/SecretDetailPanel.tsx"
Task: "T026 [US2] frontend detail tests in frontend/tests/features/secrets-dashboard/SecretDetailPanel.test.tsx"
```

## Parallel Example: User Story 3

```bash
# Parallel edit workflow tasks
Task: "T027 [US3] domain edit tests in backend/tests/SecretManager.SecretsDashboard.Domain.Tests/Services/SecretsDashboardEditTests.cs"
Task: "T028 [US3] API edit tests in backend/tests/SecretManager.SecretsDashboard.Api.Tests/Secrets/EditSecretApiTests.cs"
Task: "T031 [US3] frontend edit UX in frontend/src/features/secrets-dashboard/components/EditSecretForm.tsx"
Task: "T032 [US3] frontend edit tests in frontend/tests/features/secrets-dashboard/EditSecretForm.test.tsx"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational.
3. Complete Phase 3: User Story 1.
4. Validate grouped browse flow through Microsoft Entra ID sign-in.
5. Demo the MVP before moving to secret viewing and editing.

### Incremental Delivery

1. Finish Setup + Foundational to establish secure infrastructure.
2. Deliver User Story 1 for grouped browsing.
3. Add User Story 2 for safe secret viewing.
4. Add User Story 3 for editing with conflict protection.
5. Finish with cross-cutting hardening and documentation.

### Parallel Team Strategy

1. One developer focuses on backend domain and API foundations.
2. One developer focuses on frontend auth shell and dashboard components.
3. Once Phase 2 is done, split story work by capability while keeping contract changes synchronized.

---

## Notes

- Total tasks: 37
- Suggested MVP scope: Phase 1 + Phase 2 + User Story 1
- Every task follows the required checklist format with checkbox, task ID, optional parallel marker, required story label where applicable, and an exact file path.
- User Story task counts: US1 = 8 tasks, US2 = 6 tasks, US3 = 7 tasks.
- Setup tasks = 4, Foundational tasks = 8, Polish tasks = 4.
