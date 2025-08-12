# ✅ Agent Instructions for SUMISAN – Gestraf Architecture

Behave like a **senior C# .NET Core 6 Architect with Angular JS experience**. Your job is to help design, implement, and architect features for the **SUMISAN Wash Management System**, a warehouse web tool for surgical instrument washing.

---

## 🧩 Agent Directive for Gestraf Architecture

This project strictly follows the **Gestraf architecture**, as defined in `Gestraf_base.architecture.txt`.

You must follow these rules without exception:

✅ Use **CQRS** via **MediatR** for all business logic  
✅ Organize the solution as:

```
controlmat/
├── controlmat.Api/           # Controllers only, MediatR injection
├── controlmat.Application/   # CQRS Handlers, DTOs, Validators
├── controlmat.Domain/        # Pure Entities, Enums, Interfaces
└── controlmat.Infrastructure/# EF Core, Repositories, Config
```

✅ Controllers are thin and use:
- `IMediator.Send(...)` for commands and some queries
- Direct query injection (`IQuery`) for simple reads

✅ Register all services in `DependencyInjection.cs`

✅ Business logic, logging, and validation enforcement live **only inside Handlers**, never in controllers or services

✅ All logging is performed using `ILogger<T>` **inside handlers only**, as per `wash-logging-strategy.txt`

✅ FluentValidation is used for **field validation only**, not for business logic

---

## 🧠 Backend Capabilities and Files

| File Name                       | Description |
|--------------------------------|-------------|
| **wash-management-agent-spec.txt** | Core implementation spec — CQRS responsibilities, entity relationships, business rules |
| **wash-api-endpoints-spec.txt**   | Route structure, DTOs per endpoint, handler bindings |
| **wash-dtos-spec.txt**            | Input/output models for handlers and API |
| **wash-database-structure-spec.txt** | Complete table and FK layout, constraint enforcement |
| **wash-validation-rules.txt**     | FluentValidation rules for each DTO |
| **wash-error-codes.txt**          | HTTP status code mapping per handler |
| **wash-deployment-config.txt**    | JWT, EF Core, Swagger, Serilog setup |
| **wash-logging-strategy.txt**     | Thread-safe structured logging strategy |
| **CleanDotnetTemplate.cs**        | Minimal Gestraf-compliant CQRS app template |
| **readme_front.md**               | Angular frontend structure, routes, DTOs, flows |

---

## 🔧 Core Functional Requirements You Must Support

**Authentication**
Users authenticate via Keycloak OIDC and must supply a Bearer token on every request.

**Start Wash Flow**
- POST `/api/washing`  
- Requires: MachineId, StartUserId, ≥1 `ProtDto`  
- Optional: StartObservation  
- Generates unique `WashingId` in format `YYMMDDXX`  
- Handler: `StartWashCommand`

**Finish Wash Flow**  
- PUT `/api/washing/{id}/finish`  
- Requires: EndUserId, ≥1 Photo  
- Optional: FinishObservation  
- Sets status = `'F'`  
- Handler: `FinishWashCommand`

**Add PROT Flow**  
- POST `/api/washing/{id}/prots`  
- Adds extra PROT to in-progress wash  
- Handler: `AddProtCommand`

**Upload Photo Flow**  
- POST `/api/washing/{id}/photos`  
- Validates format, size, limit (≤99), saves image as `{ImagePath}/{Year}/{WashingId}_{XX}.jpg`  
- Handler: `UploadPhotoCommand`

**Get Active Washes**  
- GET `/api/washing/active`  
- Returns up to 2 active washes  
- Handler: `GetActiveWashesQuery`

**Get Wash By ID**  
- GET `/api/washing/{id}`  
- Returns full wash detail: PROTs, photos, timestamps  
- Handler: `GetWashByIdQuery`

---

## ⚙️ Business Rules Enforced in Handlers

| Rule | Enforced In |
|------|-------------|
| Max 2 active washes | `StartWashCommand.Handler` |
| Machine can only run one wash | `StartWashCommand.Handler` |
| Must have at least one Prot to start | `StartWashCommand.Handler` |
| Must have at least one photo to finish | `FinishWashCommand.Handler` |
| Cannot add Prot to finished wash | `AddProtCommand.Handler` |
| Max 99 photos per wash | `UploadPhotoCommand.Handler` |
| Image naming follows strict pattern | `UploadPhotoCommand.Handler` |

---

## 🧠 Frontend Awareness – Angular Integration

The Angular frontend is defined in `readme_front.md`. Be aware of:

✅ Routes:
- `/nuevo`: Start new wash
- `/finalizar/:maq`: Finish wash
- `/buscar`: Search washes
- `/perfil`: Profile

✅ DTOs:
- Use `NewWashDTO`, `FinishWashDTO` (aligned with backend)
- Prot input via manual or QR

✅ File Uploads:
- Handled via `multipart/form-data` using Angular `HttpClient`

✅ Auth:
- JWT stored in `localStorage`
- Protected routes use `AuthGuard`
- `AuthInterceptor` appends token to requests

✅ UI State:
- Signals and stores used for spinners, profile, machines, navigation

---

## ✅ Architecture Stack

- **Backend**: .NET 6 Web API  
  - MediatR for CQRS  
  - FluentValidation  
  - Serilog Logging  
  - EF Core + Migrations  
  - JWT Authentication  
  - AutoMapper (optional)

- **Frontend**: Angular 16+  
  - Signals, Services, HttpClient  
  - JWT-based route protection  
  - Multipart file uploads  
  - Tabbed shell with dynamic routing