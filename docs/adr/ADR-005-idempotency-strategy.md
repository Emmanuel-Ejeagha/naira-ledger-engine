# ADR-005: Idempotency Strategy for Financial Operations

## Context
Transfers, funding requests, and webhook processing must be idempotent to prevent duplicate charges or credits when clients retry requests.

## Problem
How to ensure that repeated execution of the same financial intent produces the same outcome without double-spending.

## Decision
Implement idempotency using unique idempotency keys (e.g., client-generated UUID or transaction reference). The strategy:
- Store idempotency key in a dedicated database table alongside the result.
- On incoming request, check if key exists; if processed, return the previously stored response.
- For operations that span multiple services, use a distributed lock (Redis) during processing to avoid race conditions.
- Idempotency keys expire after 24 hours; keys for completed transactions are kept indefinitely for audit.

## Tradeoffs
- Adds a lookup overhead for every financial request.
- Requires careful key generation and client education.
- Benefits: prevents accidental duplicates, essential for payments.

## Rejected alternatives
- **Client retry without server-side guard**: Too dangerous for money.
- **Relying solely on database unique constraints**: Only catches duplicates after inserting, may lead to unrecoverable errors.