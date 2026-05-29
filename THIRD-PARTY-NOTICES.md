# Third-Party Notices — granit-microservice-template

This file lists the third-party libraries used by
**granit-microservice-template** and their respective licenses. It is updated
whenever an external dependency is added or changed.

Last updated: 2026-03-25

---

## License summary

| License      | Package count |
| ------------ | ------------- |
| MIT          | 19            |
| Apache-2.0   | 34            |
| BSD-3-Clause | 2             |
| PostgreSQL   | 1             |

---

## Production dependencies

### MIT

| Package | Version | Copyright |
| ------- | ------- | --------- |
| Aspire.Hosting | 13.x | (c) Microsoft Corporation |
| Aspire.Hosting.AppHost | 13.x | (c) Microsoft Corporation |
| Aspire.Hosting.Keycloak | 13.x (preview) | (c) Microsoft Corporation |
| Aspire.Hosting.PostgreSQL | 13.x | (c) Microsoft Corporation |
| Aspire.Hosting.RabbitMQ | 13.x | (c) Microsoft Corporation |
| Aspire.Hosting.Redis | 13.x | (c) Microsoft Corporation |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.x | (c) Microsoft Corporation |
| Microsoft.AspNetCore.OpenApi | 10.x | (c) Microsoft Corporation |
| Microsoft.EntityFrameworkCore | 10.x | (c) Microsoft Corporation |
| Microsoft.EntityFrameworkCore.Design | 10.x | (c) Microsoft Corporation |
| Microsoft.EntityFrameworkCore.Relational | 10.x | (c) Microsoft Corporation |
| Microsoft.Extensions.Caching.StackExchangeRedis | 10.x | (c) Microsoft Corporation |
| Microsoft.Extensions.Diagnostics.HealthChecks | 10.x | (c) Microsoft Corporation |
| Microsoft.Extensions.Http.Resilience | 10.x | (c) Microsoft Corporation |
| Microsoft.Extensions.ServiceDiscovery | 10.x | (c) Microsoft Corporation |
| Scalar.AspNetCore | 2.x | Scalar Contributors |
| WolverineFx | 5.20.x | JasperFx Contributors |
| WolverineFx.Postgresql | 5.20.x | JasperFx Contributors |
| Yarp.ReverseProxy | 2.x | (c) Microsoft Corporation |

### Apache-2.0

| Package | Version | Copyright |
| ------- | ------- | --------- |
| AspNetCore.HealthChecks.NpgSql | 9.x | Copyright Xabaril Contributors |
| AspNetCore.HealthChecks.Rabbitmq | 9.x | Copyright Xabaril Contributors |
| AspNetCore.HealthChecks.Redis | 9.x | Copyright Xabaril Contributors |
| FluentValidation | 12.x | Copyright (c) Jeremy Skinner, .NET Foundation 2008-2025 |
| FluentValidation.DependencyInjectionExtensions | 12.x | Copyright (c) Jeremy Skinner, .NET Foundation 2008-2025 |
| Granit | 0.1.x (prerelease) | (c) 2025-2026 Digital Dynamics |
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
| OpenTelemetry.Exporter.OpenTelemetryProtocol | 1.15.x | Copyright The OpenTelemetry Authors |
| OpenTelemetry.Extensions.Hosting | 1.15.x | Copyright The OpenTelemetry Authors |
| OpenTelemetry.Instrumentation.AspNetCore | 1.15.x | Copyright The OpenTelemetry Authors |
| OpenTelemetry.Instrumentation.Http | 1.15.x | Copyright The OpenTelemetry Authors |

### PostgreSQL License

| Package | Version | Copyright |
| ------- | ------- | --------- |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.x | Copyright 2025 The Npgsql Development Team |

---

## Test-only dependencies

### MIT (tests)

| Package | Version | Copyright |
| ------- | ------- | --------- |
| Aspire.Hosting.Testing | 13.x | (c) Microsoft Corporation |
| Bogus | 35.x | Copyright (c) 2015 Brian Chavez |
| coverlet.collector | 8.x | (c) 2018 Toni Solarin-Sodara |
| Microsoft.NET.Test.Sdk | 18.x | (c) Microsoft Corporation |
| Testcontainers.PostgreSql | 4.x | Copyright (c) 2019-2025 Andre Hofmeister |

### Apache-2.0 (tests)

| Package | Version | Copyright |
| ------- | ------- | --------- |
| xunit.runner.visualstudio | 3.1.x | Copyright (C) .NET Foundation |
| xunit.v3 | 3.2.x | Copyright (C) .NET Foundation |

### BSD-3-Clause

| Package     | Version | Copyright                                |
| ----------- | ------- | ---------------------------------------- |
| NSubstitute | 5.x     | NSubstitute Contributors                 |
| Shouldly    | 4.x     | Copyright (c) 2017 Shouldly Contributors |

---

## Compliance notes

### Yarp.ReverseProxy

This package is used by the API Gateway (`GranitMicroservice.ApiGateway`) to
reverse-proxy frontend API calls to backend microservices. It is also used
by `Granit.Bff.Yarp` for the Backend for Frontend (BFF) pattern, which routes
requests without exposing access tokens to the browser.
