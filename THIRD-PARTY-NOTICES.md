# Third-Party Notices — granit-microservice-template

This file lists the third-party libraries used by
**granit-microservice-template** and their respective licenses. It is updated
whenever an external dependency is added or changed.

Last updated: 2026-07-05 (added Roslynator.Analyzers 4.15.0, Apache-2.0 — dev-time
code-quality analyzer, `PrivateAssets="all"`, not redistributed). Prior: 2026-06-30
(global NuGet version refresh via `dotnet restore --force-evaluate`; pinned exact
resolved versions for third-party packages following the legal-precision convention
used by granit-dotnet and granit-business; aligned the OpenTelemetry instrumentation
packages on 1.16.*. First-party Granit.* prerelease entries (0.1.x) are intentionally
left floating)

---

## License summary

| License      | Package count |
| ------------ | ------------- |
| MIT          | 28            |
| Apache-2.0   | 36            |
| BSD-3-Clause | 2             |
| PostgreSQL   | 1             |

---

## Production dependencies

### MIT

| Package | Version | Copyright |
| ------- | ------- | --------- |
| Aspire.Hosting | 13.4.6 | (c) Microsoft Corporation |
| Aspire.Hosting.AppHost | 13.4.6 | (c) Microsoft Corporation |
| Aspire.Hosting.Keycloak | 13.1.2-preview.1.26125.13 | (c) Microsoft Corporation |
| Aspire.Hosting.PostgreSQL | 13.4.6 | (c) Microsoft Corporation |
| Aspire.Hosting.RabbitMQ | 13.4.6 | (c) Microsoft Corporation |
| Aspire.Hosting.Redis | 13.4.6 | (c) Microsoft Corporation |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.9 | (c) Microsoft Corporation |
| Microsoft.AspNetCore.OpenApi | 10.0.9 | (c) Microsoft Corporation |
| Microsoft.CodeAnalysis | 5.3.0 | (c) Microsoft Corporation |
| Microsoft.CodeAnalysis.CSharp.Scripting | 5.3.0 | (c) Microsoft Corporation |
| Microsoft.CodeAnalysis.CSharp.Workspaces | 5.3.0 | (c) Microsoft Corporation |
| Microsoft.CodeAnalysis.Scripting | 5.3.0 | (c) Microsoft Corporation |
| Microsoft.CodeAnalysis.Scripting.Common | 5.3.0 | (c) Microsoft Corporation |
| Microsoft.CodeAnalysis.VisualBasic | 5.3.0 | (c) Microsoft Corporation |
| Microsoft.CodeAnalysis.VisualBasic.Workspaces | 5.3.0 | (c) Microsoft Corporation |
| Microsoft.CodeAnalysis.Workspaces.Common | 5.3.0 | (c) Microsoft Corporation |
| Microsoft.CodeAnalysis.Workspaces.MSBuild | 5.3.0 | (c) Microsoft Corporation |
| Microsoft.EntityFrameworkCore | 10.0.9 | (c) Microsoft Corporation |
| Microsoft.EntityFrameworkCore.Design | 10.0.9 | (c) Microsoft Corporation |
| Microsoft.EntityFrameworkCore.Relational | 10.0.9 | (c) Microsoft Corporation |
| Microsoft.Extensions.Caching.StackExchangeRedis | 10.0.9 | (c) Microsoft Corporation |
| Microsoft.Extensions.Diagnostics.HealthChecks | 10.0.9 | (c) Microsoft Corporation |
| Microsoft.Extensions.Http.Resilience | 10.7.0 | (c) Microsoft Corporation |
| Microsoft.Extensions.ServiceDiscovery | 10.7.0 | (c) Microsoft Corporation |
| Scalar.AspNetCore | 2.16.6 | Scalar Contributors |
| WolverineFx | 6.16.0 | JasperFx Contributors |
| WolverineFx.Postgresql | 6.16.0 | JasperFx Contributors |
| Yarp.ReverseProxy | 2.3.0 | (c) Microsoft Corporation |

### Apache-2.0

| Package | Version | Copyright |
| ------- | ------- | --------- |
| AspNetCore.HealthChecks.NpgSql | 9.0.0 | Copyright Xabaril Contributors |
| AspNetCore.HealthChecks.Rabbitmq | 9.0.0 | Copyright Xabaril Contributors |
| AspNetCore.HealthChecks.Redis | 9.0.0 | Copyright Xabaril Contributors |
| FluentValidation | 12.1.1 | Copyright (c) Jeremy Skinner, .NET Foundation 2008-2025 |
| FluentValidation.DependencyInjectionExtensions | 12.1.1 | Copyright (c) Jeremy Skinner, .NET Foundation 2008-2025 |
| Granit | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Auditing.EntityFrameworkCore | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Authentication.JwtBearer | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Authentication.JwtBearer.Keycloak | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Authorization | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Bff.Endpoints | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Bff.Yarp | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Bundle.Essentials | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Caching.StackExchangeRedis | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Diagnostics | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.EventBus | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.EventBus.Wolverine | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Http.Cors | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Http.RateLimiting | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Http.Resilience | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Identity | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Identity.Endpoints | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Identity.EntityFrameworkCore | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Identity.Keycloak | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Notifications | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Notifications.EntityFrameworkCore | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Notifications.MobilePush | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Observability | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Persistence.EntityFrameworkCore | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Persistence.EntityFrameworkCore.Hosting | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Wolverine | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| Granit.Wolverine.Postgresql | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
| OpenTelemetry.Exporter.OpenTelemetryProtocol | 1.16.0 | Copyright The OpenTelemetry Authors |
| OpenTelemetry.Extensions.Hosting | 1.16.0 | Copyright The OpenTelemetry Authors |
| OpenTelemetry.Instrumentation.AspNetCore | 1.16.0 | Copyright The OpenTelemetry Authors |
| OpenTelemetry.Instrumentation.Http | 1.16.0 | Copyright The OpenTelemetry Authors |
| Roslynator.Analyzers | 4.15.0 | Copyright (c) Josef Pihrt |

### PostgreSQL License

| Package | Version | Copyright |
| ------- | ------- | --------- |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.2 | Copyright 2025 The Npgsql Development Team |

---

## Test-only dependencies

### MIT (tests)

| Package | Version | Copyright |
| ------- | ------- | --------- |
| Aspire.Hosting.Testing | 13.4.6 | (c) Microsoft Corporation |
| Bogus | 35.6.5 | Copyright (c) 2015 Brian Chavez |
| coverlet.collector | 10.0.1 | (c) 2018 Toni Solarin-Sodara |
| Microsoft.NET.Test.Sdk | 18.7.0 | (c) Microsoft Corporation |
| Testcontainers.PostgreSql | 4.12.0 | Copyright (c) 2019-2025 Andre Hofmeister |

### Apache-2.0 (tests)

| Package | Version | Copyright |
| ------- | ------- | --------- |
| xunit.runner.visualstudio | 3.1.5 | Copyright (C) .NET Foundation |
| xunit.v3 | 3.2.2 | Copyright (C) .NET Foundation |

### BSD-3-Clause

| Package     | Version | Copyright                                |
| ----------- | ------- | ---------------------------------------- |
| NSubstitute | 5.3.0   | NSubstitute Contributors                 |
| Shouldly    | 4.3.0   | Copyright (c) 2017 Shouldly Contributors |

---

## Compliance notes

### Yarp.ReverseProxy

This package is used by the API Gateway (`GranitMicroservice.ApiGateway`) to
reverse-proxy frontend API calls to backend microservices. It is also used
by `Granit.Bff.Yarp` for the Backend for Frontend (BFF) pattern, which routes
requests without exposing access tokens to the browser.
