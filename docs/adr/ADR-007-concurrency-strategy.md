# ADR-007: Concurrency Strategy for Financial Writes

## Context
Multiple concurrent operations (funding, transfers, reversals) can target the same wallet. Without proper control, race conditions could lead to incorrect balances or double spending.

## Problem
How to prevent lost updates and ensure transactional integrity under concurrent load.

## Decision
Use a combination of optimistic concurrency control and row-level locking:
- Each wallet aggregate has a `Version` (rowversion/concurrency token) for optimistic checks.
- For high-contention operations (debiting, finalizing transfers), use `SELECT ... FOR UPDATE` (via EF Core’s `FromSqlRaw` or serializable isolation) to lock the wallet row until the transaction completes.
- All financial writes are wrapped in a database transaction with `Serializable` isolation when necessary.
- Wallet balance is derived from ledger entries, so locking ensures ledger entry insertion order.

## Tradeoffs
- Optimistic concurrency may cause retries under very high load.
- Row-level locks reduce throughput but prevent incorrect balances.
- Balance derivation from immutable ledger eliminates typical dirty reads.

## Rejected alternatives
- **Application-level locking with Redis**: Could work but introduces distributed complexity; database locks are sufficient for monolithic deployment.
- **No locking**: Unacceptable for financial integrity.