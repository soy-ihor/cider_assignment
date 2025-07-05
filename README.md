# User Management System

A full-stack user management application built with .NET 8 and Angular, following Clean Architecture principles.

## Project Structure

```
UserManagement/
├── UserManagement.Backend/     # .NET 8 Backend with Clean Architecture
│   ├── UserManagement.Domain/           # Entities and interfaces
│   ├── UserManagement.Application/      # DTOs, services, and business logic
│   ├── UserManagement.Infrastructure/  # Data access and external services
│   └── UserManagement.Web/             # API controllers and HTTP endpoints
└── UserManagement.Frontend/    # Angular Application
```

## Features

- **User CRUD Operations** - Create, Read, Update, Delete users
- **User Reordering** - Drag and drop to reorder users
- **User Import** - Import users from JSONPlaceholder API
- **Search & Filter** - Filter users by name and email
- **Pagination** - Paginated user list with validation
- **Responsive Design** - Works on desktop and mobile
- **Clean Architecture** - Proper separation of concerns

## Technology Stack

### Backend

- **.NET 8** - Web API framework
- **Entity Framework Core** - ORM with InMemory database
- **Clean Architecture** - Proper layered architecture with dependency inversion
- **CORS** - Cross-origin resource sharing enabled
- **Dependency Injection** - Proper service registration and lifetime management

### Frontend

- **Angular 17** - Frontend framework
- **Angular Material** - UI components
- **Angular CDK** - Drag and drop functionality
- **RxJS** - Reactive programming

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- Angular CLI

### Backend Setup

```bash
cd UserManagement.Backend
dotnet restore
dotnet run
```

The API will be available at `https://localhost:7001`

### Frontend Setup

```bash
cd UserManagement.Frontend
npm install
ng serve
```

The application will be available at `http://localhost:4200`

## API Endpoints

| Method   | Endpoint              | Description                                 |
| -------- | --------------------- | ------------------------------------------- |
| `GET`    | `/api/users`          | Get paginated users with optional filtering |
| `GET`    | `/api/users/{id}`     | Get user by ID                              |
| `POST`   | `/api/users`          | Create new user                             |
| `PUT`    | `/api/users/{id}`     | Update existing user                        |
| `DELETE` | `/api/users/{id}`     | Delete user (soft delete)                   |
| `PUT`    | `/api/users/reorder`  | Reorder users by drag and drop              |
| `POST`   | `/api/users/generate` | Import users from JSONPlaceholder API       |

### Query Parameters

- `name` - Filter users by name (case-insensitive)
- `email` - Filter users by email (case-insensitive)
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10, max: 100)

## Architecture

The backend follows Clean Architecture principles with proper separation of concerns:

### Domain Layer

- **Entities** - Core business objects (User)
- **Interfaces** - Repository and service contracts

### Application Layer

- **DTOs** - Data transfer objects for API communication
- **Services** - Business logic and validation
- **Extensions** - Mapping between entities and DTOs
- **Interfaces** - Service contracts

### Infrastructure Layer

- **Data** - Entity Framework context and repositories
- **Services** - External API integration and background services
- **Dependency Injection** - Service registration

### Web Layer

- **Controllers** - Thin controllers with minimal logic
- **Middleware** - CORS and exception handling
- **Extensions** - Service collection configuration

## Key Design Principles

- **Dependency Inversion** - High-level modules don't depend on low-level modules
- **Single Responsibility** - Each class has one reason to change
- **Thin Controllers** - Controllers only handle HTTP concerns
- **Rich Domain Models** - Business logic in domain entities and services
- **Proper Validation** - Input validation in application services
- **Error Handling** - Global exception middleware

## Recent Improvements

- ✅ **Clean Architecture Refactor** - Proper layer separation and naming
- ✅ **Thin Controllers** - Removed business logic from controllers
- ✅ **Service Layer Validation** - Moved validation to application services
- ✅ **Repository Pattern** - Proper data access abstraction
- ✅ **Dependency Injection** - Proper service registration
- ✅ **Code Cleanup** - Removed unused extensions and DTOs

## Development

The project uses modern .NET 8 features including:

- Top-level statements
- Record types for DTOs
- Nullable reference types
- Pattern matching
- Expression-bodied members
