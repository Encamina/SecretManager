<!--
Sync Impact Report
Version change: template -> 1.0.0
Modified principles:
- Template principle slot 1 -> I. Encrypt Secrets Before Storage
- Template principle slot 2 -> II. Microsoft Entra ID-Only Authentication
- Template principle slot 3 -> III. Library-First Delivery
Added sections:
- Security Requirements
- Delivery Workflow & Quality Gates
Removed sections:
- Unused template principle slot 4
- Unused template principle slot 5
Templates requiring updates:
- ✅ updated `.specify/templates/plan-template.md`
- ✅ updated `.specify/templates/spec-template.md`
- ✅ updated `.specify/templates/tasks-template.md`
- ✅ updated `README.md`
Follow-up TODOs:
- None
-->
# SecretManager Constitution

## Core Principles

### I. Encrypt Secrets Before Storage

All secret material MUST be encrypted before it is written to any durable store,
cache, queue, log, backup, or replica. Plaintext secrets MUST exist only in
transient memory for the minimum time required to process a request, and they MUST
never be persisted for debugging or operational convenience. Every design that
stores or transports secrets MUST identify the encryption boundary, key source,
rotation strategy, and failure mode when encryption cannot be completed.

Rationale: storage compromise is an assumed threat, so confidentiality cannot rely
on perimeter defenses alone.

### II. Microsoft Entra ID-Only Authentication

All human and service authentication MUST use Microsoft Entra ID. The system MUST
reject alternate primary authentication mechanisms, including local passwords,
shared static credentials, or parallel identity providers, unless this
constitution is formally amended. Authorization MAY use application-specific roles
or policies, but the authenticated identity MUST originate from Microsoft Entra
ID.

Rationale: a single enterprise identity source reduces credential sprawl,
centralizes access lifecycle management, and simplifies auditability.

### III. Library-First Delivery

Every feature MUST begin as an independently testable library that encapsulates the
core behavior before adapters such as CLIs, APIs, jobs, or UI layers are added.
Each library MUST declare a clear public interface, keep infrastructure concerns at
its boundaries, and ship with automated tests that exercise the library in
isolation. Delivery plans MUST show how a user story can be validated through that
library without requiring the full system to be assembled first.

Rationale: stable library seams keep secret-handling logic reusable, reviewable,
and easier to verify.

## Security Requirements

- Secret values MUST be treated as sensitive data across code, telemetry, and
  documentation.
- Logging, tracing, and error messages MUST not emit plaintext secrets or reusable
  tokens.
- Key management choices MUST support rotation and explicit failure handling;
  encryption failures MUST block persistence rather than degrade to plaintext
  storage.
- Features that persist or retrieve secrets MUST document the trust boundary
  between the caller, the encryption component, and the backing store.

## Delivery Workflow & Quality Gates

- Specifications MUST describe the storage path for any secret, the encryption
  boundary, and whether the feature introduces or changes Microsoft Entra ID flows.
- Implementation plans MUST pass a constitution check that confirms
  encryption-before-storage, Entra-only authentication, and a library-first
  decomposition with isolated tests.
- Task lists MUST include work for the core library, its isolated validation path,
  and any required security hardening for encryption and authentication.
- Pull requests MUST identify any new persistence surface for secrets and show
  evidence that plaintext is not stored.

## Governance

This constitution overrides conflicting local process notes and templates.
Amendments MUST be made in the same change set as any dependent template or
guidance update needed to keep the tooling aligned.

Governance versioning follows semantic versioning: MAJOR for incompatible
principle removals or redefinitions, MINOR for new principles or materially
expanded obligations, and PATCH for clarifications that do not change required
behavior.

Compliance reviews are mandatory for every specification, implementation plan,
task list, and pull request. Reviewers MUST verify, at minimum, that secrets are
encrypted before persistence, authentication remains Microsoft Entra ID-only, and
new behavior is implemented behind an independently testable library boundary.

**Version**: 1.0.0 | **Ratified**: 2026-03-27 | **Last Amended**: 2026-03-27
