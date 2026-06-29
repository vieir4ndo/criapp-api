# CRIApp — Backend API

REST API for the CRIApp platform, built with **.NET 8**, **C#**, and **PostgreSQL**. It handles authentication, child tracking, gamification, questionnaires, forums, notifications, and more.

## Tech Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 8 / ASP.NET Core |
| Database | PostgreSQL 16 via Entity Framework Core (Npgsql) |
| Auth | JWT + Refresh Tokens |
| Background jobs | Hangfire |
| Observability | OpenTelemetry + Azure Monitor |
| Email | Mailjet |
| SMS | Twilio |
| Cloud config (prod/stg) | Azure App Configuration + Managed Identity |
| Docs | Swagger / OpenAPI |

## Project Structure

```
src/
├── CRIApp.Api          # Entry point: controllers, middlewares, extensions, Program.cs
├── CRIApp.Application  # Services, DTOs, abstractions, background jobs
├── CRIApp.Domain       # Entities, enums, exceptions, settings
└── CRIApp.Repository   # EF Core DbContext, migrations, repositories, seeders

test/
├── CRIApp.Api.Tests
├── CRIApp.Application.Tests
└── CRIApp.Domain.Tests
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker & Docker Compose](https://www.docker.com/)

## Running locally

### 1. With Docker Compose (recommended)

```bash
docker compose up --build
```

This starts the API on port **4500** and PostgreSQL on port **5432**.

### 2. Without Docker

Start a PostgreSQL instance and set the connection string, then:

```bash
cd src/CRIApp.Api
dotnet run
```

## Configuration

All sensitive values are stored in Azure App Configuration for production and staging. For local development, set them in `appsettings.Development.json` or via environment variables:

| Key | Description |
|---|---|
| `ConnectionStrings__Postgres` | PostgreSQL connection string |
| `UserSettings__PasswordPepper` | Secret pepper for password hashing |
| `UserSettings__PasswordHashIterations` | PBKDF2 iteration count |
| `JwtTokenSettings__Key` | JWT signing key |
| `JwtTokenSettings__Issuer` | JWT issuer |
| `JwtTokenSettings__ExpiresInMinutes` | Access token lifetime |
| `JwtTokenSettings__RefreshTokenExpiresInDays` | Refresh token lifetime |
| `TwilioSettings__ClientId` | Twilio Account SID |
| `TwilioSettings__ClientSecret` | Twilio Auth Token |
| `TwilioSettings__SenderPhoneNumber` | Twilio sender number |
| `MailjetSettings__ApiKey` | Mailjet API key |
| `MailjetSettings__ApiKeySecret` | Mailjet secret |
| `MailjetSettings__SenderEmail` | Sender email address |

## Database Migrations

```bash
cd src/CRIApp.Api
dotnet ef database update
```

On first startup the application runs seeders that populate initial data: questions (HEFAS scale 10–17 months), recommendations, awards, audiovisual resources, FAQs, and a default admin user.

## Background Jobs (Hangfire)

| Job | Purpose |
|---|---|
| `AwardChildBookJob` | Assigns awards based on gamification scores |
| `UpdateChildBookScoreJob` | Recalculates child book scores |
| `SaveUsersRecommendations` | Persists personalised recommendations |
| `SendReEvaluationNotificationJob` | Notifies users to re-evaluate |
| `SendMotivationalNotificationJob` | Sends motivational push notifications |
| `SendPointsRiskNotificationJob` | Alerts when points are at risk |
| `CleanExpiredRefreshTokensJob` | Purges expired refresh tokens |
| `CleanUserResetPasswordCodesJob` | Purges expired password reset codes |

The Hangfire dashboard is available at `/hangfire` in Development.

## Running Tests

```bash
dotnet test
```

To generate a coverage report:

```bash
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

## API Endpoints (summary)

| Controller | Base path |
|---|---|
| Auth | `/api/auth` |
| User | `/api/users` |
| Child | `/api/children` |
| ChildBook | `/api/child-books` |
| Question | `/api/questions` |
| Answer | `/api/answers` |
| Forum | `/api/forums` |
| Comment | `/api/comments` |
| Notification | `/api/notifications` |
| AudiovisualResource | `/api/audiovisual-resources` |
| FAQ | `/api/faqs` |
| FeatureFlags | `/api/feature-flags` |

Full interactive documentation available at `/swagger` when running in Development.
