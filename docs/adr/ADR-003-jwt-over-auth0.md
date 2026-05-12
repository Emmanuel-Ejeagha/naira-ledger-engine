# ADR-003: JWT with ASP.NET Identity Instead of Auth0

## Context
The system requires secure authentication and authorization for Nigerian users. We considered using a third-party identity provider (Auth0) for offloading authentication.

## Problem
Which authentication strategy provides the necessary control, cost efficiency, and integration with Nigerian infrastructure?

## Decision
Use JWT tokens with ASP.NET Identity (built-in) and store user credentials in PostgreSQL. This approach:
- Avoids external dependency on Auth0’s availability and pricing model.
- Provides full control over token lifetime, refresh token rotation, and revocation.
- Supports custom claims and policy-based authorization natively.
- Simplifies local development and testing without internet reliance.

## Tradeoffs
- More responsibility for security (password hashing, token management, account lockout).
- Must implement refresh token rotation manually (but standard .NET patterns exist).
- Gain: no vendor lock-in, lower operational cost, easier compliance with local data residency.

## Rejected alternatives
- **Auth0**: Excellent developer experience but introduces third-party risk, cost scaling, and latency.
- **IdentityServer4**: Overkill for current requirements, adds complexity.