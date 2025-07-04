# User Management System

A full-stack user management application built with .NET 8 and Angular.

## Project Structure

```
UserManagement/
├── UserManagement.Backend/     # .NET 8 Web API
└── UserManagement.Frontend/    # Angular Application
```

## Features

- **User CRUD Operations** - Create, Read, Update, Delete users
- **User Reordering** - Drag and drop to reorder users
- **User Import** - Import users from JSONPlaceholder API
- **Search & Filter** - Filter users by name and email
- **Pagination** - Paginated user list
- **Responsive Design** - Works on desktop and mobile

## Technology Stack

### Backend

- **.NET 8** - Web API framework
- **Entity Framework Core** - ORM with InMemory database
- **Clean Architecture** - Domain, Application, Infrastructure, API layers
- **CORS** - Cross-origin resource sharing enabled

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

### Frontend Setup

```bash
cd UserManagement.Frontend
npm install
ng serve
```

## API Endpoints

- `GET /api/users` - Get paginated users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `PUT /api/users/reorder` - Reorder users
- `POST /api/users/generate` - Import users from JSONPlaceholder

## Architecture

The backend follows Clean Architecture principles:

- **Domain** - Entities and interfaces
- **Application** - DTOs and services
- **Infrastructure** - Data access and external services
- **API** - Controllers and HTTP endpoints
