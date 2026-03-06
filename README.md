# Task Management REST API

A **production-ready Task Management REST API** built with **ASP.NET Core 8** and **MongoDB**, following Clean Architecture principles. Deployable on Render.com.

---

## 📐 Architecture

```
TaskManagement.Api/
├── Controllers/          # HTTP layer - TasksController
├── Services/             # Business logic - ITaskService, TaskService
├── Repositories/         # Data access - ITaskRepository, TaskRepository
├── Models/               # Domain models - TaskItem
├── DTOs/                 # Data Transfer Objects - Create/Update/Response
├── Middleware/           # Cross-cutting concerns - GlobalExceptionMiddleware
├── Program.cs            # DI, pipeline, CORS, Swagger configuration
├── appsettings.json      # Base configuration
└── Dockerfile            # Docker image for Render.com
```

---

## 🚀 API Endpoints

| Method | Endpoint          | Description       |
| ------ | ----------------- | ----------------- |
| GET    | `/api/tasks`      | Get all tasks     |
| GET    | `/api/tasks/{id}` | Get task by ID    |
| POST   | `/api/tasks`      | Create a new task |
| PUT    | `/api/tasks/{id}` | Update a task     |
| DELETE | `/api/tasks/{id}` | Delete a task     |

Swagger UI is served at `/` (root).

---

## 🔧 Task Schema

```json
{
  "id": "60d0fe4f5311236168a109ca",
  "title": "Implement login page",
  "description": "Build the React login form with validation",
  "status": "Pending",
  "createdAt": "2024-01-01T12:00:00Z"
}
```

`status` values: `Pending` | `InProgress` | `Completed`

---

## ⚙️ Local Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB Atlas](https://www.mongodb.com/atlas) account (or local MongoDB)

### 1. Clone and navigate

```bash
cd TaskManagement.Api
```

### 2. Set environment variables

```bash
# Windows PowerShell
$env:MONGODB_URI = "mongodb+srv://<user>:<pass>@<cluster>.mongodb.net/"

# Linux / macOS
export MONGODB_URI="mongodb+srv://<user>:<pass>@<cluster>.mongodb.net/"
```

### 3. Build and run

```bash
dotnet build
dotnet run
```

The API starts on **http://localhost:8080**. Open your browser at http://localhost:8080 to see Swagger UI.

---

## ☁️ Deploy to Render.com

1. Push this repository to GitHub.
2. Go to [Render Dashboard](https://dashboard.render.com) → **New Web Service** → connect your repo.
3. Render will detect `render.yaml` automatically and use the **Docker** runtime.
4. Set the following **Secret Environment Variables** in the Render dashboard:

| Variable          | Description                                                          |
| ----------------- | -------------------------------------------------------------------- |
| `MONGODB_URI`     | Your MongoDB Atlas connection string                                 |
| `ALLOWED_ORIGINS` | Comma-separated allowed origins (e.g. `https://your-app.vercel.app`) |

5. Click **Deploy**. Render will build the Docker image and start the service.

---

## 🌐 CORS

- In **development**: all origins are allowed.
- In **production**: set `ALLOWED_ORIGINS` env var to restrict to your React frontend's origin.

---

## 🛡️ Error Handling

All unhandled exceptions are caught by `GlobalExceptionMiddleware` and returned as:

```json
{
  "statusCode": 500,
  "message": "An unexpected error occurred. Please try again later.",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

---

## 📦 Tech Stack

| Layer     | Technology                   |
| --------- | ---------------------------- |
| Framework | ASP.NET Core 8               |
| Database  | MongoDB (via MongoDB.Driver) |
| Docs      | Swagger / Swashbuckle        |
| Container | Docker (multi-stage)         |
| Hosting   | Render.com                   |
