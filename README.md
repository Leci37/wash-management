# 🧼 SUMISAN Wash Management System

A **warehouse web application** for managing the cleaning cycles of surgical instrument kits (PROTs) used in clinical interventions.

## 🏗️ Architecture

This project follows the **Gestraf CQRS architecture** with:
- **.NET 6 Web API** backend with MediatR, EF Core, JWT authentication
- **Angular 16+** frontend with Signals, Guards, and modern patterns
- **SQL Server 2022** database (Docker containerized)

## 🚀 Quick Start

### Prerequisites
- .NET 6 SDK
- Node.js 18+
- Docker Desktop
- SQL Server Management Studio (optional)

### Backend Setup
```bash
cd backend/src/controlmat.Api
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend Setup
```bash
cd frontend
npm install
ng serve
```

### Database Setup
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Sumisan2024!" -p 1434:1433 --name sumisan-db-dev -d mcr.microsoft.com/mssql/server:2022-latest
```

## 📁 Project Structure

```
sumisan-wash-management/
├── backend/              # .NET 6 CQRS API
│   ├── controlmat.Api/           # Controllers, JWT, Swagger
│   ├── controlmat.Application/   # Commands, Queries, DTOs
│   ├── controlmat.Domain/        # Entities, Interfaces
│   └── controlmat.Infrastructure/# EF Core, Repositories
├── frontend/             # Angular 16+ SPA
│   ├── src/app/core/            # Services, Guards, Models  
│   ├── src/app/modules/         # Feature modules
│   └── src/app/shared/          # Reusable components
├── docs/                 # Architecture & API documentation
└── database/            # SQL scripts and migrations
```

## 🔧 Core Features

- **🔐 JWT Authentication** - Secure login for warehouse users
- **🚿 Start Wash Cycles** - QR scanning or manual PROT entry
- **📸 Photo Documentation** - Upload evidence images per wash
- **✅ Finish Wash Cycles** - Complete with validation and photos
- **📊 Wash Tracking** - Search and monitor active/completed washes
- **⚙️ Business Rules** - Max 2 active washes, photo requirements

## 🎯 Status

- [x] Architecture documentation complete
- [x] Frontend UI/UX implemented  
- [ ] **CQRS Handlers implementation** *(in progress)*
- [ ] **JWT Authentication pipeline** *(in progress)*
- [ ] **Photo upload system** *(in progress)*
- [ ] Database migrations
- [ ] Integration testing

## 📚 Documentation

- [Architecture Guide](docs/architecture/)
- [API Documentation](docs/api/)
- [Database Schema](docs/database/)
- [Frontend Guide](docs/frontend/)

## 🤝 Contributing

This is an internal SUMISAN/ECNA project. Follow the Gestraf architecture patterns and CQRS principles.

---
Generated: 2025-08-11 11:41:09
