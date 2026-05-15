# NairaWalletEngine

A production‑grade, double‑entry digital wallet and ledger API for the Nigerian market (NGN). Built with .NET 10, Clean Architecture, and financial‑first design principles.

---

![CI](https://github.com/your-org/naira-ledger-engine/actions/workflows/dotnet.yml/badge.svg)

---

## 🧭 Overview

NairaWallet provides:

- **User wallets** (one per registered user) with KYC tiers
- **P2P transfers** (NGN)
- **Paystack funding** via webhooks
- **Double‑entry ledger** – every transaction is immutable and auditable
- **Idempotency** – duplicate operations are safely detected and ignored
- **Reversals** (30‑minute window)
- **Fraud velocity checks**
- **Real‑time notifications** (SignalR)
- **PDF statements**, **CSV exports**, and **QR code payments** (future layers)

---

## 🏗 Architecture

NairaWallet follows **Clean Architecture** (Hexagonal / Onion) with strict dependency direction:
WebApi (presentation)
↓
Infrastructure (persistence, caching, external services)
↓
Application (use cases, commands, queries, behaviors)
↓
Domain (entities, value objects, domain events, repository interfaces)

text

**Domain** has zero external dependencies – no MediatR, no EF Core, no ASP.NET.  
**Application** depends only on Domain, and defines interfaces for Infrastructure.  
**Infrastructure** implements those interfaces using PostgreSQL, Redis, Hangfire, etc.  
**WebApi** wires everything via dependency injection.

### Key Architectural Decisions (ADR)

All decisions are documented in [`docs/adr/`](docs/adr/):

- [ADR-001: Clean Architecture](docs/adr/ADR-001-clean-architecture.md)
- [ADR-002: Double‑Entry Ledger](docs/adr/ADR-002-double-entry-ledger.md)
- [ADR-003: JWT with ASP.NET Identity](docs/adr/ADR-003-jwt-over-auth0.md)
- [ADR-004: PostgreSQL over SQL Server](docs/adr/ADR-004-postgresql-over-sqlserver.md)
- [ADR-005: Idempotency Strategy](docs/adr/ADR-005-idempotency-strategy.md)
- [ADR-006: Webhook Design](docs/adr/ADR-006-webhook-design.md)
- [ADR-007: Concurrency Strategy](docs/adr/ADR-007-concurrency-strategy.md)

---

## 🔐 Financial Correctness

- **Double‑entry ledger**: `SUM(debits) == SUM(credits)` enforced at construction.
- **Immutable entries**: corrections only via reversal transactions.
- **Decimal precision** (`decimal`, not `float` or `double`).
- **Idempotency keys** prevent duplicate funding/transfers.
- **Concurrency safety** via optimistic locking (wallet version) and serializable transactions.
- **Negative balances** prevented unless explicitly allowed (currently not allowed).
- **Reversal window** of 30 minutes from transaction creation.

---

## 🧱 Project Structure (so far)
naira-ledger-engine/
├── backend/
│ ├── NairaWallet.sln
│ ├── Directory.Build.props
│ ├── global.json
│ ├── .editorconfig
│ ├── NairaWallet.Domain/ # ✅ Completed (Layer 1)
│ │ ├── Aggregates/ (Wallet, Transaction)
│ │ ├── Entities/ (LedgerEntry)
│ │ ├── ValueObjects/ (Money, IdempotencyKey, TransactionReference, WalletTag, UserId)
│ │ ├── Enums/ (TransactionType, TransactionStatus, LedgerEntryDirection, KycLevel)
│ │ ├── DomainEvents/ (WalletCreated, TransferCompleted, FraudCheckTriggered, ...)
│ │ └── Interfaces/ (IWalletRepository, ITransactionRepository)
│ ├── NairaWallet.Application/ # ✅ Completed (Layer 2)
│ │ ├── Commands/ (CreateWallet, FundWallet, Transfer, ReverseTransaction)
│ │ ├── Queries/ (GetWalletBalance, GetTransactionHistory)
│ │ ├── Behaviors/ (Idempotency, Validation)
│ │ ├── EventHandlers/ (WalletCreated, TransferCompleted, FraudCheck)
│ │ ├── Interfaces/ (IIdempotencyStore, ILedgerQueryService, IUnitOfWork, ...)
│ │ └── DTOs/
│ ├── NairaWallet.Infrastructure/ # 🔨 Next (Layer 3)
│ ├── NairaWallet.WebApi/ # 🔨 After Infrastructure (Layer 4)
│ └── NairaWallet.Tests/
│ ├── Domain/ # ✅ Full coverage
│ └── Application/ # ✅ Full coverage
├── frontend/ # 🔜 React + TypeScript (later)
├── docs/
│ ├── adr/ # ✅ All ADRs written
│ ├── api/ # 🔜 Swagger / Postman
│ └── diagrams/ # 🔜 Sequence, C4, ERD
├── docker-compose.yml # ✅ PostgreSQL + Redis + WebApi placeholder
├── .github/workflows/dotnet.yml # ✅ CI pipeline
└── README.md

text

---

## 🚦 Current Status

**Layer 1 – Domain** ✅  
**Layer 2 – Application** ✅  

- All domain aggregates, value objects, and invariants implemented and tested.
- All command handlers, queries, pipeline behaviors (idempotency, validation), and domain event handlers implemented and tested.
- No `TODO`s or placeholder code – every handler is fully compilable and testable.

**Next:** Layer 3 – Infrastructure (PostgreSQL, Redis, EF Core, Hangfire)

---

## ⚙️ Tech Stack

| Layer            | Technology                               |
|------------------|------------------------------------------|
| Backend          | .NET 10, ASP.NET Core Minimal APIs      |
| Architecture     | Clean Architecture (Domain, Application, Infrastructure, WebApi) |
| Database         | PostgreSQL (EF Core 10)                  |
| Caching / Locks  | Redis (StackExchange.Redis)             |
| Background Jobs  | Hangfire (PostgreSQL storage)           |
| Auth             | JWT + ASP.NET Identity                  |
| Validation       | FluentValidation                        |
| Mediator         | MediatR 12                              |
| Testing          | xUnit, FluentAssertions, Moq, Testcontainers |
| Monitoring       | Serilog, OpenTelemetry readiness (later) |
| Frontend         | React + TypeScript (Vite)               |

---

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK (10.0.100+)
- Docker & Docker Compose
- Node.js 20+ (for frontend, later)

### Clone & Build

```bash
git clone https://github.com/your-org/naira-ledger-engine.git
cd naira-ledger-engine

# Build backend
cd backend
dotnet build

# Run tests
dotnet test
Run infrastructure only (for local development)
bash
# Start PostgreSQL and Redis
docker-compose up -d postgres redis

# The WebApi is not yet wired – you can still explore the Domain and Application layers via tests.
📘 API Documentation (coming in Layer 4)
Once the WebApi is implemented, Swagger UI will be available at:

http://localhost:8080/swagger

Example endpoints:

text
POST   /api/v1/wallets          (create wallet)
GET    /api/v1/wallets/{id}     (get wallet)
POST   /api/v1/wallets/{id}/fund (fund wallet)
POST   /api/v1/transfers        (P2P transfer)
POST   /api/v1/transfers/{id}/reverse
GET    /api/v1/transactions?cursor=...&pageSize=20
A Postman collection will be provided in docs/api/.

🛡 Financial Safeguards
Idempotency Keys: All fund and transfer commands require a unique key. Repeating the same key returns the original result without double‑posting.

Double‑Entry Balancing: Transactions are constructed with mandatory debit and credit entries; unbalanced transactions throw an InvalidOperationException.

Reversal Window: Only transactions within 30 minutes can be reversed; reversal attempts after that are rejected.

Balance Derivation: Wallet balance is computed from ledger entries, not stored as a mutable field – preventing drift.

Concurrency: Optimistic locking (Version token) and serializable isolation will be added in Infrastructure.

All rules are enforced at the Domain level and cannot be bypassed.

📊 Roadmap
Layer 0 – Repository bootstrap, project structure, ADRs, CI/CD

Layer 1 – Domain layer with aggregates, value objects, domain events, and tests

Layer 2 – Application layer with commands, queries, idempotency, validation, and tests

Layer 3 – Infrastructure layer (EF Core, PostgreSQL, Redis, Hangfire, Identity, webhook processing)

Layer 4 – WebApi layer (Minimal APIs, JWT auth, Rate limiting, SignalR, Health checks)

Frontend – React SPA (login, wallet, transfers)

Diagrams – C4, sequence, ERD

Postman collection & API docs

Docker full stack – production Dockerfiles and compose

Deployment – Azure/AWS ready

🤝 Contributing
Please follow Clean Architecture rules when adding features. All financial code must be tested with xUnit. See .editorconfig for style conventions.

📄 License
MIT

