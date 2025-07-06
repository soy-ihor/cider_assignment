# User Management System

A full-stack user management application built with .NET 8 and Angular, following Clean Architecture principles.

## Features

- **User CRUD Operations** - Create, Read, Update, Delete users
- **User Reordering** - Drag and drop to reorder users
- **User Import** - Import users from JSONPlaceholder API
- **Search & Filter** - Filter users by name and email
- **Pagination** - Paginated user list with validation
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

## Setup Instructions

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

The API will be available at:

- HTTP: `http://localhost:5171`
- Swagger UI: `http://localhost:5171/swagger`

### Frontend Setup

```bash
cd UserManagement.Frontend
npm install
ng serve
```

The application will be available at `http://localhost:4200`

## API Endpoints

| Method   | Endpoint                                                               | Description                                 |
| -------- | ---------------------------------------------------------------------- | ------------------------------------------- |
| `GET`    | `/api/users?name={name}&email={email}&page={page}&pageSize={pageSize}` | Get paginated users with optional filtering |
| `GET`    | `/api/users/{id}`                                                      | Get user by ID                              |
| `POST`   | `/api/users`                                                           | Create new user                             |
| `PUT`    | `/api/users/{id}`                                                      | Update existing user                        |
| `DELETE` | `/api/users/{id}`                                                      | Delete user (soft delete)                   |
| `PUT`    | `/api/users/reorder`                                                   | Reorder users by drag and drop              |
| `POST`   | `/api/users/generate`                                                  | Import users from JSONPlaceholder API       |

**Query Parameters for GET /api/users:**

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

## Implementation Assumptions

1. **UI Framework:** Used Angular Material for consistent and accessible UI components
2. **State Management:** Used simple service-based state management (no NgRx for this scope)
3. **Form Validation:** Implemented reactive forms with custom validators
4. **Error Handling:** Used snackbar notifications for user feedback
5. **Drag & Drop:** Used Angular CDK for drag and drop functionality

## Time Spent Breakdown

- **Backend Development:** 2.5 hours
- **Frontend Development:** 2.5 hours
- **General Setup & Documentation:** 1 hour

**Total Time: 6 hours**

## Challenges Encountered and Solutions

The main challenge was the 3-hour time constraint for this assignment. To meet the deadline and deliver a complete, functional application, CursorAI was used to accelerate the development process by generating the initial project skeleton, basic CRUD operations, and foundational code structure. This allowed for faster iteration and focus on implementing the core features and Clean Architecture principles within the limited timeframe.
