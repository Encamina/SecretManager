---

description: "Task list template for feature implementation"
---

# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Automated tests are REQUIRED for each independently testable library. Add contract and integration tests whenever the feature introduces external interfaces or the specification requires them.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Single project**: `src/`, `tests/` at repository root with core libraries under `src/lib/`
- **Web app**: `backend/src/`, `frontend/src/`
- **Mobile**: `api/src/`, `ios/src/` or `android/src/`
- Paths shown below assume single project - adjust based on plan.md structure

<!--
  ============================================================================
  IMPORTANT: The tasks below are SAMPLE TASKS for illustration purposes only.

  The /speckit.tasks command MUST replace these with actual tasks based on:
  - User stories from spec.md (with their priorities P1, P2, P3...)
  - Feature requirements from plan.md
  - Entities from data-model.md
  - Endpoints from contracts/

  Tasks MUST be organized by user story so each story can be:
  - Implemented independently
  - Tested independently
  - Delivered as an MVP increment

  DO NOT keep these sample tasks in the generated tasks.md file.
  ============================================================================
-->

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and the initial library-first feature skeleton

- [ ] T001 Create project structure per implementation plan
- [ ] T002 Initialize [language] project with [framework] dependencies
- [ ] T003 [P] Establish the core library module and public interface in `src/lib/`
- [ ] T004 [P] Configure the automated test harness for isolated library validation

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

Examples of foundational tasks (adjust based on your project):

- [ ] T005 Define the encryption boundary and key management integration for persisted secrets
- [ ] T006 [P] Implement Microsoft Entra ID authentication and authorization plumbing
- [ ] T007 [P] Setup logging and error handling that redact or omit plaintext secrets
- [ ] T008 Create persistence abstractions that reject plaintext-secret writes
- [ ] T009 Document environment/configuration requirements for Entra ID and encryption keys

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - [Title] (Priority: P1) 🎯 MVP

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own through the library boundary]

### Tests for User Story 1 (REQUIRED)

> Add isolated library tests that validate this story through the core library. Add contract or integration tests when the story introduces an external interface.

- [ ] T010 [P] [US1] Library test for [capability] in `tests/unit/test_[name].py`
- [ ] T011 [P] [US1] Contract or integration test for [adapter/journey] in `tests/integration/test_[name].py`

### Implementation for User Story 1

- [ ] T012 [P] [US1] Create or update library types in `src/lib/[feature]/[file].py`
- [ ] T013 [US1] Implement story behavior in `src/lib/[feature]/[file].py`
- [ ] T014 [US1] Add adapter entry point in `src/[location]/[file].py`
- [ ] T015 [US1] Enforce encryption-before-storage or Entra identity propagation where this story needs it
- [ ] T016 [US1] Add secret-safe logging, metrics, and documentation updates

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - [Title] (Priority: P2)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own through the library boundary]

### Tests for User Story 2 (REQUIRED)

- [ ] T017 [P] [US2] Library test for [capability] in `tests/unit/test_[name].py`
- [ ] T018 [P] [US2] Contract or integration test for [adapter/journey] in `tests/integration/test_[name].py`

### Implementation for User Story 2

- [ ] T019 [P] [US2] Create or update library types in `src/lib/[feature]/[file].py`
- [ ] T020 [US2] Implement story behavior in `src/lib/[feature]/[file].py`
- [ ] T021 [US2] Add adapter entry point in `src/[location]/[file].py`
- [ ] T022 [US2] Integrate with shared encryption or Entra components as needed

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - [Title] (Priority: P3)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own through the library boundary]

### Tests for User Story 3 (REQUIRED)

- [ ] T023 [P] [US3] Library test for [capability] in `tests/unit/test_[name].py`
- [ ] T024 [P] [US3] Contract or integration test for [adapter/journey] in `tests/integration/test_[name].py`

### Implementation for User Story 3

- [ ] T025 [P] [US3] Create or update library types in `src/lib/[feature]/[file].py`
- [ ] T026 [US3] Implement story behavior in `src/lib/[feature]/[file].py`
- [ ] T027 [US3] Add adapter entry point in `src/[location]/[file].py`
- [ ] T028 [US3] Integrate with shared encryption or Entra components as needed

**Checkpoint**: All user stories should now be independently functional

---

[Add more user story phases as needed, following the same pattern]

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] TXXX [P] Documentation updates in `docs/` or `README.md`
- [ ] TXXX Verify plaintext secrets are never logged or persisted
- [ ] TXXX Review Microsoft Entra ID scopes, claims, and authorization mappings
- [ ] TXXX [P] Additional unit, contract, or integration tests in `tests/`
- [ ] TXXX Security hardening and key rotation validation
- [ ] TXXX Run quickstart.md validation

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1 but should be independently testable
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - May integrate with US1/US2 but should be independently testable

### Within Each User Story

- Library types and isolated tests before adapter polish
- Core library behavior before transport or UI adapters
- Secret persistence safeguards before any storage write path ships
- Story complete before moving to the next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- All tests for a user story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# Launch isolated validation tasks for User Story 1 together:
Task: "Library test for [capability] in tests/unit/test_[name].py"
Task: "Contract or integration test for [adapter/journey] in tests/integration/test_[name].py"

# Launch library work for User Story 1 together:
Task: "Create or update library types in src/lib/[feature]/[file].py"
Task: "Implement story behavior in src/lib/[feature]/[file].py"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently through the library boundary
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo
5. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable through the library boundary
- Review encryption and authentication impacts whenever a story touches storage or identity
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, and cross-story dependencies that break independence
