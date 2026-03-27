# SecretManager

SecretManager is a secret management solution for software applications.

## Operating Principles

- Secrets are encrypted before they reach any persistent storage surface.
- Authentication is exclusively through Microsoft Entra ID.
- Every feature starts as an independently testable library before adapters are added.

The authoritative governance rules live in `.specify/memory/constitution.md`.
