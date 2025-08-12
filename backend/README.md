# 🧱 SUMISAN Backend (.NET 6 CQRS API)

> **Current Status**: 🚧 **SCAFFOLDED** - Basic project structure created, CQRS implementation pending

## 📋 Current Implementation State

### ✅ What's Already Set Up
- [x] **Solution Structure** - Gestraf-compliant 4-layer architecture
- [x] **Project References** - Proper dependencies between layers
- [x] **Basic API Project** - With Program.cs, WeatherForecast example
- [x] **Docker Support** - Dockerfile and .dockerignore configured
- [x] **Development Environment** - appsettings.json structure ready

### ❌ What Needs Implementation
- [ ] **Domain Layer** - Entities (User, Washing, Prot, Photo, Machine, Parameter)
- [ ] **Application Layer** - CQRS Commands, Queries, DTOs, Validators
- [ ] **Infrastructure Layer** - EF Core DbContext, Repositories, Migrations
- [ ] **API Controllers** - Replace WeatherForecast with Washing, Auth controllers
- [ ] **Authentication** - JWT implementation
- [ ] **Database Setup** - EF Core migrations and seeding
- [ ] **Business Logic** - All wash management rules and validation

## 🏗️ Architecture Overview

```
backend/
├── controlmat.sln                    # Solution file
├── src/
│   ├── controlmat.Api/              # 🧱 API Layer (Controllers, JWT, Swagger)
│   │   ├── Controllers/
│   │   │   └── WeatherForecastController.cs  # ⚠️ PLACEHOLDER - Replace with real controllers
│   │   ├── appsettings.json         # ✅ Configuration structure ready
│   │   ├── Program.cs              # ⚠️ BASIC SETUP - Needs MediatR, JWT, EF Core
│   │   └── Dockerfile              # ✅ Docker support ready
│   │
│   ├── controlmat.Application/      # 🧠 CQRS Layer (Commands, Queries, DTOs)
│   │   └── Class1.cs               # ❌ EMPTY - Needs full CQRS implementation
│   │
│   ├── controlmat.Domain/           # 🏛️ Domain Layer (Entities, Interfaces)
│   │   └── Class1.cs               # ❌ EMPTY - Needs domain models
│   │
│   └── controlmat.Infrastructure/   # 🗄️ Infrastructure Layer (EF Core, Repos)
│       └── Class1.cs               # ❌ EMPTY - Needs DbContext and repositories
```

## 🎯 Implementation Roadmap

### Phase 1: Domain Foundation
```csharp
// controlmat.Domain/Entities/
- User.cs          # UserId, UserName, Role
- Machine.cs       # Id, Name  
- Washing.cs       # WashingId, MachineId, UserIds, Status, Dates
- Prot.cs          # WashingId, ProtId, BatchNumber, BagNumber
- Photo.cs         # WashingId, FileName, FilePath
- Parameter.cs     # Name, Value (for ImagePath config)

// controlmat.Domain/Interfaces/
- IWashingRepository.cs
- IPhotoRepository.cs  
- IUserRepository.cs
```

### Phase 2: Infrastructure Setup
```csharp
// controlmat.Infrastructure/
- ControlmatDbContext.cs      # EF Core context
- Repositories/               # Repository implementations
- DependencyInjection.cs      # Service registration
- Migrations/                 # Database migrations
```

### Phase 3: CQRS Implementation
```csharp
// controlmat.Application/Common/
├── Commands/Washing/
│   ├── StartWashCommand.cs          # Begin new wash cycle
│   ├── FinishWashCommand.cs         # Complete wash with validation
│   ├── AddProtCommand.cs            # Add PROT to existing wash
│   └── UploadPhotoCommand.cs        # Store wash evidence photos
├── Queries/Washing/
│   ├── GetActiveWashesQuery.cs      # List in-progress washes
│   └── GetWashByIdQuery.cs          # Get wash details with photos/prots
├── Dto/
│   ├── NewWashDto.cs               # Start wash request
│   ├── FinishWashDto.cs            # Finish wash request
│   └── WashingResponseDto.cs       # Wash details response
└── Validators/
    ├── NewWashValidator.cs         # FluentValidation rules
    └── FinishWashValidator.cs      # Business rule validation
```

### Phase 4: API Controllers
```csharp
// controlmat.Api/Controllers/
- WashingController.cs        # Main wash management endpoints
- AuthController.cs           # JWT authentication
- PhotoController.cs          # File upload handling (optional)
```

## 🔧 Required NuGet Packages

Based on `sumisan-dotnet6-dependencies.txt`:

### controlmat.Api
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="6.0.25" />
<PackageReference Include="NSwag.AspNetCore" Version="14.1.0" />
```

### controlmat.Application  
```xml
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="FluentValidation" Version="11.9.2" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
<PackageReference Include="MediatR" Version="12.3.0" />
<PackageReference Include="Serilog" Version="4.0.0" />
<PackageReference Include="Serilog.AspNetCore" Version="6.1.0" />
```

### controlmat.Infrastructure
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="6.0.25" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.25" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="6.0.25" />
<PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="6.0.0" />
```

### controlmat.Domain
```xml
<PackageReference Include="MediatR" Version="12.3.0" />
```

## 🗄️ Database Configuration

### Connection String (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1434;Database=SUMISAN;User Id=SA;Password=Sumisan2024!;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": "your-super-secret-jwt-key-minimum-32-characters",
    "Issuer": "sumisan-api", 
    "Audience": "sumisan-client",
    "ExpiryMinutes": 480
  },
  "ImagePath": "/shared/photos"
}
```

### Docker Database Setup
```bash
# Start SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Sumisan2024!" \
  -p 1434:1433 --name sumisan-db-dev \
  -d mcr.microsoft.com/mssql/server:2022-latest

# Test connection
sqlcmd -S localhost,1434 -U SA -P "Sumisan2024!" -Q "SELECT @@VERSION"
```

## 🚀 Development Setup

### Prerequisites
- .NET 6 SDK
- SQL Server (Docker container provided)
- Visual Studio 2022 or VS Code

### Quick Start
```bash
# 1. Install dependencies (run this script)
./install-dependencies.ps1

# 2. Set up database
dotnet ef migrations add InitialCreate --project src/controlmat.Infrastructure --startup-project src/controlmat.Api
dotnet ef database update --project src/controlmat.Infrastructure --startup-project src/controlmat.Api

# 3. Run the API
cd src/controlmat.Api
dotnet run
```

### Swagger UI
Once running, visit: https://localhost:7001/swagger

## 🎯 Key Business Rules to Implement

| Rule | Location | Implementation |
|------|----------|----------------|
| Max 2 active washes | `StartWashCommand.Handler` | Count active washes before creating |
| Machine availability | `StartWashCommand.Handler` | Check if machine already has active wash |
| ≥1 PROT to start | `StartWashCommand.Handler` | Validate ProtEntries array not empty |
| ≥1 Photo to finish | `FinishWashCommand.Handler` | Query photos count for washing |
| WashingId format | `StartWashCommand.Handler` | Generate YYMMDDXX format |
| Photo naming | `UploadPhotoCommand.Handler` | Enforce {WashingId}_{XX}.jpg pattern |

## 🔐 Authentication

All authentication is handled by Keycloak using the OAuth2/OIDC protocol. Clients obtain a Bearer token from Keycloak and include it in the `Authorization` header for all protected API calls.

## 📝 Logging Strategy

Using Serilog with structured logging in each Handler:

```csharp
public class Handler : IRequestHandler<Request, WashDto>
{
    private readonly ILogger<Handler> _logger;
    
    public async Task<WashDto> Handle(Request request, CancellationToken ct)
    {
        _logger.LogInformation("🌀 StartWash - STARTED. Input: {@Request}", request);
        
        try
        {
            // Business logic here
            var result = new WashDto();
            _logger.LogInformation("✅ StartWash - COMPLETED. Output: {@Result}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ StartWash - ERROR. Input: {@Request}", request);
            throw;
        }
    }
}
```

## 🤝 Development Guidelines

1. **Follow Gestraf Architecture** - Keep business logic in Handlers only
2. **Use MediatR** - All operations via `IMediator.Send()`  
3. **Thin Controllers** - Only route, validate, and delegate
4. **Repository Pattern** - Abstract data access in Domain interfaces
5. **Validation Strategy** - FluentValidation for syntax, Handlers for business rules
6. **Error Handling** - Consistent HTTP status codes and structured responses

## 📚 Related Documentation

- [Architecture Guide](../docs/architecture/) - Gestraf CQRS patterns
- [API Endpoints](../docs/api/) - Complete endpoint specifications  
- [Database Schema](../docs/database/) - Table structure and relationships
- [Frontend Integration](../frontend/README.md) - Angular client setup

---

**Ready for Implementation!** 🚀 Follow the roadmap above to transform this scaffold into a complete CQRS-based wash management system.