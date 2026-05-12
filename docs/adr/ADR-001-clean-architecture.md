# ADR-001: Clean Architecture Adoption

## Context
We are building a production-grade digital wallet and ledger system. Financial correctness, auditability, and long-term maintainability are paramount. The system must be testable, resistant to corruption, and allow independent evolution of business rules from infrastructure.

## Problem
How to structure the codebase to enforce separation of concerns, protect domain invariants, and facilitate testing without coupling to frameworks or external systems.

## Decision
Adopt Clean Architecture (also known as Hexagonal/Onion architecture) with strict dependency rules:
- **Domain** layer contains pure business logic, entities, value objects, and domain events. Zero external dependencies.
- **Application** layer orchestrates use cases and depends only on Domain.
- **Infrastructure** layer implements persistence, external APIs, and cross-cutting concerns; depends on Domain and Application.
- **WebApi** layer is the composition root; references Application and Infrastructure only for DI wiring.

Dependency direction: Domain ← Application ← Infrastructure ← WebApi (runtime), but compile-time references follow a controlled pattern.

## Tradeoffs
- More projects and stricter layering increases initial setup complexity.
- Requires discipline to avoid leaking framework concerns into Domain.
- Benefits: high testability, easy to swap infrastructure implementations, strong safeguard against accidental coupling.

## Rejected alternatives
- **Traditional N-tier**: Domain would be anemic and tightly coupled to EF Core, making testing harder.
- **Vertical slices without domain isolation**: Risks business logic scattering across handlers, weaker invariant enforcement.

## Operational impact
- New developers must learn the dependency rules.
- Code reviews must ensure no Domain dependency violations.
- Simpler to add new features or change database providers later.