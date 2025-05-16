# Zion Reminder API

A RESTful API for managing reminders and tasks, built with ASP.NET Core.

## Features

- Create, read, update, and delete reminders
- Mark reminders as complete
- Filter reminders by status and priority
- In-memory database for quick development and testing

## API Endpoints

| Method | Endpoint                 | Description                      |
|--------|--------------------------|----------------------------------|
| GET    | /api/reminders           | Get all reminders                |
| GET    | /api/reminders/{id}      | Get a specific reminder          |
| POST   | /api/reminders           | Create a new reminder            |
| PUT    | /api/reminders/{id}      | Update an existing reminder      |
| PATCH  | /api/reminders/{id}/complete | Mark a reminder as complete  |
| DELETE | /api/reminders/{id}      | Delete a reminder                |

## Getting Started

1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet run` to start the API
4. Access the Swagger UI at `https://localhost:7108/swagger`

## Technologies

- ASP.NET Core 9.0
- Entity Framework Core
- In-memory database (for development)
- Swagger/OpenAPI