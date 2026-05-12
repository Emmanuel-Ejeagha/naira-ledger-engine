# ADR-004: PostgreSQL Over SQL Server

## Context
The system needs a relational database for financial data, user identity, and event storage. We evaluated SQL Server and PostgreSQL.

## Problem
Choose a database that balances ACID compliance, developer ecosystem, operational cost, and compatibility with .NET.

## Decision
Use PostgreSQL. Reasons:
- Open-source, no licensing fees; ideal for Nigerian startups and production deployments.
- Excellent support for JSONB, full-text search, and advanced indexing.
- Superior handling of concurrent writes and row-level locking.
- Npgsql EF Core provider is mature and performant.
- Native support for `decimal` without precision issues.

## Tradeoffs
- Some tooling differences from SQL Server (SSMS) - mitigated by DBeaver/pgAdmin.
- Slightly different SQL dialect occasionally requires query adaptation.
- Overall: cost-effective, robust, and widely adopted in fintech.

## Rejected alternatives
- **SQL Server**: Licensing costs and vendor lock-in; less flexible for scaling on Linux/Docker.
- **MySQL**: Weaker JSON support and less strict ACID default behavior.