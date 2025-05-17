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

> **Note:** For email functionality using Gmail, you need to set up an app password. See the [Configuration](#configuration) section below.

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/zion-reminder-api.git
cd zion-reminder-api
```

### 2. Start the PostgreSQL Database with Docker

```bash
docker-compose up -d postgres pgadmin
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
# Configure secret settings
dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password-here"

# Run the API
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

| Method | Endpoint                              | Description                                 |
|--------|---------------------------------------|---------------------------------------------|
| GET    | /api/reminders                        | Get all reminders                           |
| GET    | /api/reminders/{id}                   | Get a specific reminder                     |
| POST   | /api/reminders                        | Create a new reminder                       |
| PUT    | /api/reminders/{id}                   | Update an existing reminder                 |
| PATCH  | /api/reminders/{id}/complete          | Mark a reminder as complete                 |
| DELETE | /api/reminders/{id}                   | Delete a reminder                           |
| POST   | /api/events/send-to-reviewer          | Create reviewer event and schedule notifications for each recipient |

---


### Send to Reviewer Endpoint

**POST** `/api/events/send-to-reviewer`

Creates a reviewer event and schedules multiple notifications for each recipient in `ForEmails` according to the calculated schedule.

**Headers:**

```
Authorization: Bearer <your_token>
Content-Type: application/json
```

**Request Body Example:**
```json
{
  "RequestedBy": {
    "Name": "Manager Name",
    "Email": "manager@example.com"
  },
  "RequestedFor": {
    "Name": "Employee Name",
    "Email": "employee@example.com"
  },
  "Reviewers": [
    { "Name": "Alice", "Email": "alice@example.com" },
    { "Name": "Bob", "Email": "bob@example.com" }
  ],
  "Attempt": 5,
  "ApplicationLink": "https://example.com/app",
  "EndDate": "2025-05-30T23:59:59Z"
}
```

**Field Descriptions:**

| Field        | Type           | Required | Description                                                      |
|--------------|----------------|----------|------------------------------------------------------------------|
| RequestedBy  | Person         | Yes      | The user who is requesting the review                            |
| RequestedFor | Person         | Yes      | The user for whom the review is being requested                  |
| Reviewers    | Person[]       | Yes      | List of reviewer users (at least one required)                   |
| Attempt      | int?           | No       | Number of notification attempts per reviewer (if not set, uses config default) |
| ApplicationLink | string      | No       | Link to the application (optional)                               |
| EndDate      | datetime?      | Yes      | End date for the event; used to calculate notification schedule  |

**Behavior:**
- If `Attempt` is not provided, the default value from config (`Reviewer:DefaultAttempt`) is used.
- For each reviewer in `Reviewers`, notifications are scheduled according to the calculated schedule between the current time and `EndDate` (number of notifications = Attempt).
- Each notification is created for the corresponding reviewer (using their name and email from the `Person` object).

**Example Success Response:**
```json
{
  "success": true,
  "message": "Reviewer event and notifications created successfully"
}
```


## Authorization Usage (JWT)

### 1. Get a JWT Token

Send a POST request to the token endpoint:

```
POST http://localhost:5243/api/auth/token
```

Example response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

You can use tools like Postman, Swagger UI, or curl for this step.

---

### 2. Call a Protected Endpoint with the Token


For example, to call the `send-to-tm` endpoint:

```
POST http://localhost:5243/api/events/send-to-tm
```

Add the following HTTP header:

```
Authorization: Bearer <your_token>
```

Example request body:
```json
{
  "TalentManager": {
    "Name": "John Doe",
    "Email": "john.doe@example.com"
  },
  "Talent": {
    "Name": "Jane Smith",
    "Email": "jane.smith@example.com"
  },
  "By": {
    "Name": "System",
    "Email": "system@example.com"
  },
  "ApplicationLink": "https://example.com",
  "StartDate": "2025-05-17T09:00:00Z",
  "EndDate": "2025-05-30T23:59:59Z"
}
```


**Field Descriptions:**

| Field            | Type     | Required | Description                                      |
|------------------|----------|----------|--------------------------------------------------|
| TalentManager    | Person   | Yes      | The talent manager (recipient of the notification) |
| Talent           | Person   | Yes      | The employee for whom the notification is sent    |
| By               | Person   | Yes      | The user who is sending the notification          |
| ApplicationLink  | string   | Yes      | Link to the application                           |
| StartDate        | datetime | Yes      | Start date for the event                          |
| EndDate          | datetime | Yes      | End date for the event                            |

If the token is valid, you will receive a response like:
```json
{
  "success": true,
  "message": "Event created successfully"
}
```

---

### 3. Example in Postman

1. First, send a POST request to `/api/auth/token` and copy the `token` from the response.
2. Create a new POST request to `/api/events/send-to-tm`.
3. In the Headers tab, add:
   - `Authorization: Bearer <your_token>`
4. In the Body tab, select `raw` and `JSON`, then paste your request body.
5. Send the request. If the token is missing or invalid, you will get a 401 Unauthorized error.

#### Example images:
![Postman POST token](https://user-images.githubusercontent.com/6388707/232232964-2e2e2e2e-2e2e-2e2e-2e2e-2e2e2e2e2e2e.png)
![Postman Bearer Token](https://user-images.githubusercontent.com/6388707/232233012-3e3e3e3e-3e3e-3e3e-3e3e-3e3e3e3e3e3e.png)
![Postman 401](https://user-images.githubusercontent.com/6388707/232233045-4e4e4e4e-4e4e-4e4e-4e4e-4e4e4e4e4e4e.png)

---

## Configuration

### Email Settings

The API includes email notification functionality that requires SMTP configuration. There are several ways to configure email settings:

#### 1. Development Environment - User Secrets

For local development, use .NET User Secrets to store sensitive information:

```bash
# Initialize user secrets
dotnet user-secrets init

# Set the email password
dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password"
```

#### 2. Production Environment - Environment Variables

For production deployment, use environment variables:

```bash
# PowerShell
$env:EMAIL_PASSWORD = "your-gmail-app-password"

# Linux/macOS
export EMAIL_PASSWORD="your-gmail-app-password"
```

#### 3. Docker Environment

When using Docker, you can:

1. Create a `.env` file from the `.env.sample` template:
   ```bash
   cp .env.sample .env
   # Edit .env with your actual values
   ```

2. Or provide environment variables directly:
   ```bash
   docker-compose up -d -e EMAIL_PASSWORD="your-gmail-app-password"
   ```

> **⚠️ Important:** For Gmail, you need to:
> 1. Enable 2-Step Verification on your Google account
> 2. Generate an "App Password" from your Google Account security settings
> 3. Use that app password instead of your regular Google password

For more details on environment variable configuration, see [deployment-environment-variables.md](deployment-environment-variables.md).

---

## Technologies

- ASP.NET Core 9.0
- Entity Framework Core
- PostgreSQL
- MailKit/MimeKit (Email)
- Docker & Docker Compose
- Swagger/OpenAPI