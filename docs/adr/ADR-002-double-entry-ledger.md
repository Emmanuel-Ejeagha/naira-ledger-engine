# ADR-002: Double-Entry Ledger as Source of Truth

## Context
The wallet must maintain an accurate, auditable record of all financial movements. Regulatory compliance and reconciliation require immutable, balanced transaction records.

## Problem
How to ensure every financial operation preserves the fundamental accounting equation (debits = credits) and prevents data corruption.

## Decision
Implement a double-entry ledger where every transaction consists of at least two ledger entries (debit and credit). The wallet balance is derived from the ledger, not stored as a mutable field. All monetary operations go through the ledger, and the system enforces:
- SUM(debits) == SUM(credits) within each transaction.
- Ledger entries are immutable; corrections occur via reversal entries.
- Wallet balance is computed as SUM(credits) - SUM(debits) for a wallet’s ledger accounts.

## Tradeoffs
- Querying balance requires aggregation (optimized via materialized views or caching).
- Write operations are more complex due to mandatory balancing.
- Gain: tamper-proof financial history, audit trail, and fraud resistance.

## Rejected alternatives
- **Single-entry ledger**: Simpler but vulnerable to drift, missing controls, and non-compliance.
- **Storing balance directly on wallet**: Risks concurrency anomalies and undetected inconsistencies.