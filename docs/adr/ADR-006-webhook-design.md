# ADR-006: Webhook Processing for Paystack Integration

## Context
NairaWallet will accept NGN funding via Paystack. Paystack sends webhook events for successful charges. These must be processed reliably and idempotently.

## Problem
How to handle webhooks in a way that guarantees funds are credited exactly once, even if Paystack resends the same event.

## Decision
Design a webhook processing pipeline:
1. Receive webhook with signature verification.
2. Store raw payload in an `InboundWebhook` table with a unique `EventId`.
3. Enqueue a background job (Hangfire) for processing.
4. The job validates the webhook, checks idempotency, creates ledger entries, and updates wallet.
5. Return 200 OK quickly to Paystack; actual processing runs asynchronously.
6. Use Redis to cache processed event IDs for faster duplicate detection.

## Tradeoffs
- Asynchronous processing means the user sees funded balance after a short delay.
- Requires robust retry logic and dead-letter handling.
- Gain: resilience to Paystack retries, clear audit trail.

## Rejected alternatives
- **Synchronous processing in webhook controller**: Risk of timeout and duplicate processing on retries.
- **No event store**: Would lose traceability of incoming events.