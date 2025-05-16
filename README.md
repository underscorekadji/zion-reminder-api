# Zion Reminder API

A RESTful API for managing reminders and tasks, built with ASP.NET Core and PostgreSQL.

## Features

- Create, read, update, and delete reminders
- Mark reminders as complete
- Filter reminders by status and priority
- PostgreSQL database running in Docker

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Running Locally

Follow these steps to run the API on your local machine:

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/zion-reminder-api.git
cd zion-reminder-api
```

### 2. Start the PostgreSQL Database with Docker

```bash
docker-compose up -d
```

This command starts the PostgreSQL database and pgAdmin in detached mode. You can verify they're running with:

```bash
docker ps
```

### 3. Apply Database Migrations

Ensure the Entity Framework Core tools are installed:

```bash
dotnet tool install --global dotnet-ef
```

Then apply the migrations:

```bash
dotnet ef database update
```

### 4. Run the API

```bash
dotnet run
```

The API will be accessible at:
- HTTP: http://localhost:5243
- HTTPS: https://localhost:7108

### 5. Explore the API

- Swagger UI: https://localhost:7108/ (or http://localhost:5243/)
- pgAdmin: http://localhost:5050 (email: admin@example.com, password: admin)

## Development Workflow

### Installing Entity Framework Core Tools

If you get the error "Could not execute because the specified command or file was not found" when trying to run EF Core commands, you need to install the EF Core CLI tools:

```bash
# Install the EF Core CLI tools globally
dotnet tool install --global dotnet-ef

# If already installed, ensure it's up-to-date
dotnet tool update --global dotnet-ef
```

Verify the installation with:
```bash
dotnet ef --version
```

### Creating New Migrations

After making changes to the database models:

```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

If you receive an error about the design package, add the Microsoft.EntityFrameworkCore.Design package to your project:
```bash
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### Building the Project

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

## API Endpoints

| Method | Endpoint                 | Description                      |
|--------|--------------------------|----------------------------------|
| GET    | /api/reminders           | Get all reminders                |
| GET    | /api/reminders/{id}      | Get a specific reminder          |
| POST   | /api/reminders           | Create a new reminder            |
| PUT    | /api/reminders/{id}      | Update an existing reminder      |
| PATCH  | /api/reminders/{id}/complete | Mark a reminder as complete  |
| DELETE | /api/reminders/{id}      | Delete a reminder                |

## Technologies

- ASP.NET Core 9.0
- Entity Framework Core
- PostgreSQL
- Docker & Docker Compose
- Swagger/OpenAPI