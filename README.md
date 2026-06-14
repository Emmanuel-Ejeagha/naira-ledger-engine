# NairaLedger

A production‑grade, double‑entry digital wallet and ledger platform for the Nigerian market (NGN).  
Built with .NET 10, Clean Architecture, React + TypeScript, and financial‑first design principles.

---

## 🧭 Overview

NairaLedger provides:

- **User wallets** (one per registered user) with KYC tiers
- **P2P transfers** (NGN) with detailed receipts and reversal eligibility
- **Paystack funding** via webhooks (idempotent, signature‑verified)
- **Double‑entry ledger** – immutable, auditable, balances derived from ledger
- **Idempotency** – duplicate operations are safely detected and ignored
- **Reversals** (30‑minute window)
- **Fraud velocity checks** – automatic wallet freeze
- **Real‑time notifications** (SignalR) with persistent storage and toast popups
- **Email alerts** for debits and credits
- **CSV & PDF exports** (client‑side CSV, PDF with jsPDF)
- **QR code payments** (generate, download, share)
- **Full KYC flow** (submit, approve/reject by admin)
- **Change password**, profile management
- **Admin panel** – KYC approval, transaction reversal, create admin users

---

## 🏗 Architecture

NairaLedger follows **Clean Architecture** (Hexagonal / Onion) with strict dependency direction:

```
WebApi (presentation)
    ↓
Infrastructure (persistence, caching, external services)
    ↓
Application (use cases, commands, queries, behaviours)
    ↓
Domain (entities, value objects, domain events, repository interfaces)
```

**Domain** has zero external dependencies – no MediatR, no EF Core, no ASP.NET.  
**Application** depends only on Domain.  
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
- **Decimal precision** (`decimal(18,2)`), never `float` or `double`.
- **Idempotency keys** prevent duplicate funding/transfers.
- **Concurrency safety** via optimistic locking (wallet version) and serializable transactions when necessary.
- **Negative balances** prevented unless explicitly allowed.
- **Reversal window** of 30 minutes from transaction creation.
- **Fraud velocity checks** automatically freeze wallets on high‑risk patterns.

---

## 🧱 Project Structure

```
naira-ledger-engine/
├── backend/
│   ├── NairaLedger.sln
│   ├── Directory.Build.props
│   ├── global.json
│   ├── .editorconfig
│   ├── NairaLedger.Domain/               # ✅ Completed
│   │   ├── Aggregates/                   (Wallet, Transaction)
│   │   ├── Entities/                     (LedgerEntry)
│   │   ├── ValueObjects/                 (Money, IdempotencyKey, TransactionReference, WalletTag, UserId)
│   │   ├── Enums/                        (TransactionType, TransactionStatus, LedgerEntryDirection, KycLevel)
│   │   ├── DomainEvents/                 (WalletCreated, TransferCompleted, FraudCheckTriggered, WalletFrozen, ...)
│   │   └── Interfaces/                   (IWalletRepository, ITransactionRepository)
│   ├── NairaLedger.Application/          # ✅ Completed
│   │   ├── Commands/                     (CreateWallet, FundWallet, Transfer, ReverseTransaction, Auth, KYC, Admin)
│   │   ├── Queries/                      (GetWalletBalance, GetTransactionHistory)
│   │   ├── Behaviors/                    (Idempotency, Validation)
│   │   ├── EventHandlers/                (WalletCreated, TransferCompleted, FraudCheck, WalletFrozen)
│   │   ├── Interfaces/                   (IIdempotencyStore, ILedgerQueryService, IUnitOfWork, IPaystackService, IPaymentGateway, IUserService, IEmailService, IRealTimeNotifier, ...)
│   │   ├── DTOs/                         (TransactionDto, PagedResponse)
│   │   └── Exceptions/                   (UserAlreadyExistsException)
│   ├── NairaLedger.Infrastructure/       # ✅ Completed
│   │   ├── Persistence/                  (NairaLedgerDbContext, EF Configurations, Repositories, UnitOfWork, Migrations)
│   │   ├── Identity/                     (AppUser, AppRole)
│   │   ├── Services/                     (IdempotencyStore, LedgerQueryService, TransactionQueryService, FraudDetectionService, PaystackService, PaystackPaymentGateway, EmailService, NotificationService, JwtTokenService, RedisRefreshTokenStore, UserService)
│   │   ├── Outbox/                       (OutboxMessage, OutboxPublisherJob)
│   │   ├── HealthChecks/
│   │   └── DependencyInjection.cs
│   ├── NairaLedger.WebApi/               # ✅ Completed
│   │   ├── Program.cs
│   │   ├── Middleware/                   (ExceptionHandlingMiddleware, CorrelationIdMiddleware)
│   │   ├── Endpoints/                   (Auth, Wallets, Transfers, Transactions, KYC, Admin, Webhooks)
│   │   ├── Hubs/                         (NotificationHub)
│   │   ├── Services/                     (SignalRRealTimeNotifier)
│   │   └── Authorization/               (Policies, AuthorizeCheckOperationFilter)
│   └── NairaLedger.Tests/                # ✅ Full coverage
│       ├── Domain/
│       ├── Application/
│       └── Infrastructure/
├── frontend/                             # ✅ Completed
│   ├── src/
│   │   ├── api/                          (Axios client, funding, transfers, kyc, admin, auth)
│   │   ├── components/                   (UI components, layout, SignalRProvider)
│   │   ├── features/                     (auth, dashboard, wallet, funding, transfer, transactions, kyc, notifications, qr, settings, admin)
│   │   ├── hooks/                        (useSignalR, useUserWallet)
│   │   ├── stores/                       (authStore, notificationStore)
│   │   └── ... 
│   ├── vite.config.ts
│   ├── tailwind.config.ts
│   └── ...
├── docs/
│   ├── adr/                              # ✅ All ADRs written
│   ├── api/                              # ✅ Swagger
│   └── diagrams/                         # ✅ Sequence, C4, ERD
├── docker-compose.yml
├── docker-compose.override.yml
├── .github/workflows/dotnet.yml
└── README.md
```

---

## 🚦 Current Status

| Layer             | Status      |
|-------------------|-------------|
| **Domain**        | ✅ Complete |
| **Application**   | ✅ Complete |
| **Infrastructure**| ✅ Complete |
| **WebApi**        | ✅ Complete |
| **Tests**         | ✅ Full coverage (Domain, Application, Infrastructure integration tests) |
| **Frontend**      | ✅ Complete (React + TypeScript, all features) |

All layers are production‑ready. Zero placeholders. Zero TODOs.

---

## ⚙️ Tech Stack

| Layer            | Technology                               |
|------------------|------------------------------------------|
| Backend          | .NET 10, ASP.NET Core Minimal APIs      |
| Architecture     | Clean Architecture (Domain, Application, Infrastructure, WebApi) |
| Database         | PostgreSQL (Render, EF Core 10)          |
| Caching / Locks  | Redis (Upstash, StackExchange.Redis)    |
| Background Jobs  | Hangfire (PostgreSQL storage)           |
| Auth             | JWT + ASP.NET Identity                  |
| Validation       | FluentValidation                        |
| Mediator         | MediatR 12                              |
| Testing          | xUnit, FluentAssertions, Moq, Testcontainers |
| Monitoring       | Serilog, Health checks (PostgreSQL, Redis, Hangfire), cron-job.org for uptime |
| Rate Limiting    | Built‑in ASP.NET Core Rate Limiting     |
| Notifications    | SignalR (Long Polling)                  |
| Payment Gateway  | Paystack (HTTP client)                 |
| Email            | SMTP                                    |
| Frontend         | React 19, TypeScript, Vite, TanStack Query, Zustand, React Hook Form, Zod, Tailwind CSS, Shadcn/UI, Sonner |
| Frontend Hosting | Vercel                                   |
| Backend Hosting  | Render                                   |
| Documentation    | Swagger / OpenAPI                       |

---

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK (10.0.100+)
- Docker & Docker Compose
- Node.js 20+ (for frontend)

### Clone & Run with Docker (Backend only)

```bash
git clone https://github.com/Emmanuel-Ejeagha/naira-ledger-engine.git
cd naira-ledger-engine/backend

# Start the full stack (PostgreSQL, Redis, WebApi)
docker-compose up -d --build
```

The API will be available at:
- **HTTP**: `http://localhost:8080/swagger`
- **HTTPS**: `https://localhost:8081/swagger`

Migrations and seed data (admin user, system bank wallet) are applied automatically.

### Run Backend without Docker (local development)

```bash
cd backend

# Start PostgreSQL and Redis manually (or use Docker just for them)
docker-compose up -d postgres redis

# Apply migrations
dotnet ef database update --project NairaLedger.Infrastructure --startup-project NairaLedger.WebApi

# Run the API
dotnet run --project NairaLedger.WebApi
```

Swagger: `http://localhost:5000/swagger` (or your configured port)

### Run Frontend

```bash
cd frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:3000`. It proxies API calls to the backend (default `https://localhost:8081`). Adjust the proxy target in `vite.config.ts` if your backend runs on a different port.

### Run Tests

```bash
cd backend
dotnet test
```

---

## 🌐 Deployed Application

| Component   | URL |
|-------------|-----|
| **Frontend** | [https://naira-ledger-engine.vercel.app](https://naira-ledger-engine.vercel.app) |
| **Backend API** | [https://naira-ledger-engine-1.onrender.com/swagger](https://naira-ledger-engine-1.onrender.com/swagger) |
| **Health Check** | [https://naira-ledger-engine-1.onrender.com/health/live](https://naira-ledger-engine-1.onrender.com/health/live) |

> The Render backend is kept alive 24/7 using **cron-job.org**, which pings the health endpoint every 5 minutes to prevent the free tier from sleeping.

---

## 📘 API Documentation & Testing

Swagger UI is available at the above URLs. All endpoints are documented with summaries, descriptions, and response types.

### 🔑 Step‑by‑Step Testing Guide (Swagger / Postman)

#### 1. Register a user
- `POST /api/v1/auth/register`
- Body: `{ "email": "demo@nairawallet.ng", "fullName": "Demo User", "password": "Demo@1234" }`
- Save the `walletId`.

#### 2. Login
- `POST /api/v1/auth/login` with same credentials.
- Copy the `accessToken`.

#### 3. Authorise Swagger
- Click **Authorize** → enter `Bearer <accessToken>`.

#### 4. Check wallet balance
- `GET /api/v1/wallets/{walletId}/balance`

#### 5. Initiate Paystack funding
- `POST /api/v1/wallets/{walletId}/fund`
- Body: `{ "amount": 5000, "callbackUrl": "https://example.com" }`
- Returns an authorization URL; in production redirect the user.

#### 6. Simulate webhook (funds credit)
- `POST /api/v1/webhooks/paystack` with Paystack event payload and signature. After processing, balance increases.

#### 7. P2P transfer
- Register a second user, copy their `walletId`.
- `POST /api/v1/transfers`
- Body: `{ "fromWalletId": "...", "toWalletId": "...", "amount": 500, "idempotencyKey": "unique-key-001" }`

#### 8. Transaction history
- `GET /api/v1/transactions?walletId=...&pageSize=10`

#### 9. KYC submission & admin approval
- `POST /api/v1/kyc/submit` (user)
- `POST /api/v1/kyc/approve` (admin: `admin@nairawallet.ng` / `Admin123!`)

#### 10. Real‑time notifications
- Open two browsers, log in as two different users.
- Transfer money between them – both will see a toast and a persistent notification in the Notification Center.

#### 11. Health checks
- Liveness: `GET /health/live`
- Readiness: `GET /health/ready`

A Postman collection will be provided in `docs/api/`.

---

## 🛡 Financial Safeguards

- All monetary operations use `decimal(18,2)`.
- Transaction construction enforces `SUM(debits) == SUM(credits)`.
- Idempotency keys prevent double spending; stored in DB + Redis cache.
- Wallet balance derived from immutable ledger – never stored as mutable field.
- Optimistic concurrency (`Version` token) prevents lost updates.
- Fraud velocity checks freeze wallets automatically on high‑risk patterns.
- Reversals only allowed within 30 minutes.
- All financial writes are atomic and auditable.
- Webhook processing is idempotent and signature‑verified.

---

## 📊 Roadmap

- [x] Domain layer
- [x] Application layer
- [x] Infrastructure layer (EF Core, Redis, Hangfire, Identity, Paystack)
- [x] WebApi layer (Minimal APIs, JWT, Rate Limiting, SignalR, Health Checks)
- [x] Frontend (React SPA with all features)
- [x] Real‑time notifications (SignalR) + persistent storage
- [x] Admin panel (KYC approval, reversal, user management)
- [x] Deployed frontend (Vercel)
- [x] Deployed backend (Render)
- [ ] Enhanced monitoring (OpenTelemetry, Grafana)
- [ ] PDF statements, CSV exports, QR payments (partially implemented)
- [ ] Deployment scripts (Azure/AWS)

---

## 🤝 Contributing

Please follow Clean Architecture rules. All financial code must be tested with xUnit. See `.editorconfig` for style conventions.

---

## 📄 License

MIT
```