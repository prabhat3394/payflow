# PayFlow — Solution & Folder Structure

## Repository layout

```
payflow/
├── README.md
├── .gitignore
├── docker-compose.yml              ← run all services locally together
├── docker-compose.override.yml     ← local dev overrides (ports, env vars)
│
├── docs/
│   ├── architecture.md             ← system design write-up (link in README)
│   ├── adr/                        ← Architecture Decision Records
│   │   ├── 001-cqrs-with-mediatr.md
│   │   ├── 002-saga-for-transfers.md
│   │   └── 003-outbox-pattern.md
│   └── diagrams/
│       └── system-architecture.png
│
├── src/
│   ├── ApiGateway/                 ← YARP reverse proxy, routes to services
│   │   ├── PayFlow.ApiGateway.csproj
│   │   ├── Program.cs
│   │   └── appsettings.json        ← YARP route config
│   │
│   ├── Services/
│   │   ├── Identity/
│   │   │   └── PayFlow.Identity.Api/
│   │   │       ├── Controllers/
│   │   │       │   └── AuthController.cs
│   │   │       ├── Application/
│   │   │       │   ├── Commands/
│   │   │       │   │   ├── RegisterUserCommand.cs
│   │   │       │   │   └── LoginCommand.cs
│   │   │       │   └── Queries/
│   │   │       │       └── GetUserProfileQuery.cs
│   │   │       ├── Domain/
│   │   │       │   └── Entities/
│   │   │       │       └── AppUser.cs
│   │   │       ├── Infrastructure/
│   │   │       │   ├── Persistence/
│   │   │       │   │   └── IdentityDbContext.cs
│   │   │       │   └── Services/
│   │   │       │       └── TokenService.cs
│   │   │       ├── Dockerfile
│   │   │       └── PayFlow.Identity.Api.csproj
│   │   │
│   │   ├── Wallet/
│   │   │   └── PayFlow.Wallet.Api/
│   │   │       ├── Controllers/
│   │   │       │   └── WalletController.cs
│   │   │       ├── Application/
│   │   │       │   ├── Commands/
│   │   │       │   │   ├── CreditWalletCommand.cs
│   │   │       │   │   └── DebitWalletCommand.cs
│   │   │       │   └── Queries/
│   │   │       │       └── GetBalanceQuery.cs
│   │   │       ├── Domain/
│   │   │       │   ├── Entities/
│   │   │       │   │   └── Wallet.cs
│   │   │       │   └── Events/
│   │   │       │       └── WalletDebitedEvent.cs
│   │   │       ├── Infrastructure/
│   │   │       │   ├── Persistence/
│   │   │       │   │   └── WalletDbContext.cs
│   │   │       │   └── Repositories/
│   │   │       │       └── WalletRepository.cs
│   │   │       ├── Dockerfile
│   │   │       └── PayFlow.Wallet.Api.csproj
│   │   │
│   │   ├── Transaction/
│   │   │   └── PayFlow.Transaction.Api/
│   │   │       ├── Controllers/
│   │   │       │   └── TransactionController.cs
│   │   │       ├── Application/
│   │   │       │   ├── Commands/
│   │   │       │   │   └── InitiateTransferCommand.cs
│   │   │       │   ├── Sagas/
│   │   │       │   │   └── TransferSaga.cs         ← MassTransit saga
│   │   │       │   └── Queries/
│   │   │       │       └── GetTransactionHistoryQuery.cs
│   │   │       ├── Domain/
│   │   │       │   ├── Entities/
│   │   │       │   │   └── Transaction.cs
│   │   │       │   └── Enums/
│   │   │       │       └── TransactionStatus.cs
│   │   │       ├── Infrastructure/
│   │   │       │   ├── Persistence/
│   │   │       │   │   └── TransactionDbContext.cs
│   │   │       │   └── Outbox/
│   │   │       │       └── OutboxProcessor.cs      ← background service
│   │   │       ├── Dockerfile
│   │   │       └── PayFlow.Transaction.Api.csproj
│   │   │
│   │   ├── Notification/
│   │   │   └── PayFlow.Notification.Api/
│   │   │       ├── Hubs/
│   │   │       │   └── NotificationHub.cs          ← SignalR hub
│   │   │       ├── Consumers/
│   │   │       │   └── TransactionCompletedConsumer.cs
│   │   │       ├── Services/
│   │   │       │   └── EmailService.cs             ← Azure Communication Services
│   │   │       ├── Dockerfile
│   │   │       └── PayFlow.Notification.Api.csproj
│   │   │
│   │   └── Reporting/
│   │       └── PayFlow.Reporting.Api/
│   │           ├── Functions/
│   │           │   └── GenerateMonthlyStatement.cs ← Azure Function (timer trigger)
│   │           ├── GrpcServices/
│   │           │   └── ReportingGrpcService.cs     ← gRPC server
│   │           ├── Protos/
│   │           │   └── reporting.proto
│   │           ├── Dockerfile
│   │           └── PayFlow.Reporting.Api.csproj
│   │
│   └── Shared/
│       ├── PayFlow.Shared.Contracts/               ← DTOs, events, shared types
│       │   ├── Events/
│       │   │   ├── TransactionInitiatedEvent.cs
│       │   │   ├── TransactionCompletedEvent.cs
│       │   │   └── TransactionFailedEvent.cs
│       │   └── Dtos/
│       │       ├── WalletDto.cs
│       │       └── TransactionDto.cs
│       │
│       └── PayFlow.Shared.Infrastructure/          ← cross-cutting concerns
│           ├── Middleware/
│           │   └── ExceptionHandlingMiddleware.cs
│           ├── Extensions/
│           │   └── ServiceCollectionExtensions.cs
│           └── Outbox/
│               └── OutboxMessage.cs
│
├── frontend/
│   └── payflow-web/                ← Angular 17 SPA
│       ├── src/
│       │   ├── app/
│       │   │   ├── core/
│       │   │   │   ├── auth/
│       │   │   │   │   ├── auth.service.ts
│       │   │   │   │   ├── auth.guard.ts
│       │   │   │   │   └── auth.interceptor.ts     ← attaches JWT
│       │   │   │   └── signalr/
│       │   │   │       └── notification.service.ts
│       │   │   ├── features/
│       │   │   │   ├── dashboard/
│       │   │   │   ├── wallet/
│       │   │   │   ├── transfer/
│       │   │   │   └── transactions/
│       │   │   └── store/                          ← NgRx
│       │   │       ├── wallet/
│       │   │       │   ├── wallet.actions.ts
│       │   │       │   ├── wallet.reducer.ts
│       │   │       │   └── wallet.effects.ts
│       │   │       └── transactions/
│       │   └── environments/
│       │       ├── environment.ts
│       │       └── environment.prod.ts
│       └── package.json
│
├── infra/
│   ├── bicep/                      ← Infrastructure as Code
│   │   ├── main.bicep              ← entry point, calls modules
│   │   ├── modules/
│   │   │   ├── app-service.bicep
│   │   │   ├── sql-database.bicep
│   │   │   ├── service-bus.bicep
│   │   │   ├── key-vault.bicep
│   │   │   └── app-insights.bicep
│   │   └── parameters/
│   │       ├── dev.parameters.json
│   │       └── prod.parameters.json
│   └── scripts/
│       └── deploy.sh
│
└── .azure/
    └── pipelines/
        ├── ci.yml                  ← build + test on every PR
        └── cd.yml                  ← deploy to Azure on merge to main
```

---

## Key architectural decisions

### Clean Architecture per service
Every service follows the same internal layering:
- `Domain` — entities, value objects, domain events. No external dependencies.
- `Application` — CQRS handlers (MediatR), business logic, interfaces.
- `Infrastructure` — EF Core, repositories, external services, outbox.
- `Controllers` — thin HTTP entry points, only call MediatR.

### Communication patterns
| From → To | Protocol | Why |
|---|---|---|
| Angular SPA → API Gateway | REST + JWT | Standard web API |
| API Gateway → Services | REST (internal) | Simple routing |
| Transaction → Wallet | gRPC | Low-latency balance check before debit |
| Services → Notification | Azure Service Bus | Fire-and-forget, decoupled |
| Notification → Angular | SignalR | Real-time push |

### Database per service
Each service has its own Azure SQL database. They never share a database or call each other's DB directly. This is non-negotiable for microservices.

### The Outbox pattern (most interview-worthy piece)
The Transaction service writes both the transaction record and an outbox message in a single DB transaction. A background `OutboxProcessor` service reads unprocessed messages and publishes them to Service Bus. This guarantees at-least-once delivery even if Service Bus is temporarily unavailable.

```
1. BEGIN TRANSACTION
2.   INSERT INTO Transactions (...)
3.   INSERT INTO OutboxMessages (event_type, payload, processed = false)
4. COMMIT
5. [Background] OutboxProcessor polls OutboxMessages WHERE processed = false
6. [Background] Publishes to Service Bus → marks processed = true
```

This is one of the best things you can explain in an interview. Most candidates have never implemented it.

---

## Running locally

```bash
# Start all services + databases via Docker Compose
docker-compose up -d

# Run Angular frontend
cd frontend/payflow-web
npm install && ng serve

# App available at http://localhost:4200
# API Gateway at http://localhost:5000
```

---

## Week-by-week build order

| Week | What you build |
|---|---|
| 1 | Identity service + JWT auth + Key Vault integration |
| 2 | Wallet service (CQRS + Repository) + Transaction service (Saga + Outbox) |
| 3 | Deploy Identity + Wallet to Azure App Service, wire Key Vault |
| 4 | Notification service (SignalR + Service Bus) + Azure DevOps CI/CD pipeline |
| 5 | Reporting service (Azure Functions + gRPC) + App Insights |
| 6 | Angular: auth flow, wallet dashboard, transfer form |
| 7 | Angular: NgRx state, real-time notifications via SignalR, transaction history |
| 8 | Polish README, record Loom walkthrough, submit to jobs |
