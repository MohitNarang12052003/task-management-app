# TaskFlow — Full-Stack Task Management App

A production-ready Task Management app built with **ASP.NET Core 8** + **React + Vite** + **MongoDB Atlas**, with **JWT authentication**, deployable on **Render.com** for free.

---

## 📋 Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Prerequisites](#prerequisites)
3. [MongoDB Atlas Setup](#mongodb-atlas-setup)
4. [Local Development](#local-development)
5. [Deploy to Render.com](#deploy-to-rendercom)
6. [API Reference](#api-reference)
7. [Troubleshooting](#troubleshooting)

---

## Architecture Overview

```
┌─────────────────────┐        ┌──────────────────────┐        ┌──────────────┐
│   React Frontend    │  HTTP  │  ASP.NET Core 8 API  │ Driver │ MongoDB Atlas│
│  (Vite Static Site) │◄──────►│  JWT Auth + CRUD     │◄──────►│   (Free M0)  │
│  Render Static Site │        │  Render Web Service  │        │ cloud.mongodb│
└─────────────────────┘        └──────────────────────┘        └──────────────┘
```

### Project Structure

```
Render App Deployment/
├── render.yaml
├── .gitignore
├── PREREQUISITES.md
├── README.md
├── TaskManagement.Api/                ← ASP.NET Core 8 API
│   ├── Controllers/
│   │   ├── AuthController.cs          ← Register / Login
│   │   └── TasksController.cs         ← CRUD (JWT protected)
│   ├── Services/
│   │   ├── AuthService.cs             ← BCrypt + JWT generation
│   │   └── TaskService.cs
│   ├── Repositories/
│   │   ├── UserRepository.cs
│   │   └── TaskRepository.cs          ← Per-user queries
│   ├── Models/
│   │   ├── User.cs
│   │   └── TaskItem.cs
│   ├── DTOs/                          ← Request/Response shapes
│   ├── Middleware/GlobalExceptionMiddleware.cs
│   ├── Program.cs
│   └── Dockerfile
└── Frontend/                          ← React + Vite
    ├── src/
    │   ├── App.jsx                    ← Task UI (auth-gated)
    │   ├── AuthPage.jsx               ← Login / Register
    │   ├── AuthContext.jsx            ← Global auth state
    │   ├── api.js                     ← Fetch client + Bearer token
    │   ├── useToast.js
    │   └── index.css
    └── .env                           ← VITE_API_URL
```

---

## Prerequisites

See **[PREREQUISITES.md](./PREREQUISITES.md)** for full details. Summary:

| Tool                  | Version | Purpose          |
| --------------------- | ------- | ---------------- |
| .NET SDK              | 8.0+    | Run the backend  |
| Node.js               | 18+ LTS | Run the frontend |
| Git                   | Any     | Push to GitHub   |
| MongoDB Atlas account | —       | Cloud database   |
| GitHub account        | —       | Host code        |
| Render.com account    | —       | Free deployment  |

---

## MongoDB Atlas Setup

1. Sign up at [cloud.mongodb.com](https://cloud.mongodb.com)
2. Create a **free M0 cluster**
3. **Create a database user** → Security → Database Access → Add New User (Password auth)
4. **Get your connection string** → Clusters → Connect → Drivers → C# / .NET → Toggle Show Password
5. **Allow all IPs** → Security → Network Access → Add IP Address → `0.0.0.0/0`

Your connection string looks like:

```
mongodb+srv://USERNAME:PASSWORD@cluster0.xxxxx.mongodb.net/?retryWrites=true&w=majority
```

---

## Local Development

### 1. Clone / Download the project

```bash
git clone https://github.com/YOUR_USERNAME/YOUR_REPO.git
```

### 2. Run the Backend API

> **Windows — open PowerShell:**

```powershell
# If 'dotnet' is not recognized after installing .NET 8:
$env:PATH += ";C:\Program Files\dotnet"

# Required environment variables
$env:MONGODB_URI = "mongodb+srv://USER:PASS@cluster0.xxxxx.mongodb.net/?retryWrites=true&w=majority"
$env:JWT_SECRET  = "your-secret-key-at-least-32-characters-long!"

cd TaskManagement.Api
dotnet run
```

API starts at: **http://localhost:8080**  
Swagger UI (interactive docs): **http://localhost:8080**

> ⚠️ **All three must be set in the same terminal session before running.**

### 3. Run the Frontend

> **Open a second terminal:**

```powershell
cd Frontend
npm install
npm run dev
```

Frontend starts at: **http://localhost:5173**

You'll see a **Login / Register** screen. Register an account to start using the app.

### 4. Frontend API URL config

`Frontend/.env`:

```env
VITE_API_URL=http://localhost:8080
```

---

## Deploy to Render.com

### Step 1 — Push to GitHub

```powershell
cd "Render App Deployment"
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO.git
git branch -M main
git push -u origin main
```

> 💡 If Git uses the wrong GitHub account, use a Personal Access Token:
>
> ```powershell
> git remote set-url origin https://USERNAME:TOKEN@github.com/USERNAME/REPO.git
> ```

---

### Step 2 — Deploy the Backend API

1. [dashboard.render.com](https://dashboard.render.com) → **New + → Web Service**
2. Connect GitHub → select your repo
3. Configure:

   | Field          | Value                |
   | -------------- | -------------------- |
   | Root Directory | `TaskManagement.Api` |
   | Runtime        | **Docker**           |
   | Instance Type  | Free                 |

4. Add Environment Variables:

   | Key                      | Value                                |
   | ------------------------ | ------------------------------------ |
   | `MONGODB_URI`            | Your MongoDB Atlas connection string |
   | `JWT_SECRET`             | A long random string (min 32 chars)  |
   | `ASPNETCORE_ENVIRONMENT` | `Production`                         |

5. Click **Deploy Web Service** → ⭐ Copy your API URL

---

### Step 3 — Deploy the Frontend

1. Render → **New + → Static Site**
2. Select same repo
3. Configure:

   | Field             | Value           |
   | ----------------- | --------------- |
   | Root Directory    | `Frontend`      |
   | Build Command     | `npm run build` |
   | Publish Directory | `dist`          |

4. Add Environment Variable:

   | Key            | Value                                |
   | -------------- | ------------------------------------ |
   | `VITE_API_URL` | Your API URL **(no trailing slash)** |

5. Click **Create Static Site**

---

### Step 4 — Enable CORS

Render → API Service → Environment → Add:

| Key               | Value             |
| ----------------- | ----------------- |
| `ALLOWED_ORIGINS` | Your frontend URL |

---

### Step 5 — Verify

Open your frontend Render URL → Register → Create tasks → Logout → Login again. ✅

---

## API Reference

Base URL: `http://localhost:8080` (local) or your Render URL (production)

### Auth Endpoints (public)

| Method | Endpoint             | Description        | Body                |
| ------ | -------------------- | ------------------ | ------------------- |
| `POST` | `/api/auth/register` | Create new account | `{email, password}` |
| `POST` | `/api/auth/login`    | Sign in            | `{email, password}` |

**Auth response:**

```json
{ "token": "eyJ...", "email": "you@example.com", "userId": "65f..." }
```

Store the `token` and send it as `Authorization: Bearer <token>` on all task requests.

---

### Task Endpoints (🔒 JWT required)

| Method   | Endpoint          | Description        | Body                           |
| -------- | ----------------- | ------------------ | ------------------------------ |
| `GET`    | `/api/tasks`      | Get all your tasks | —                              |
| `GET`    | `/api/tasks/{id}` | Get task by ID     | —                              |
| `POST`   | `/api/tasks`      | Create task        | `{title, description, status}` |
| `PUT`    | `/api/tasks/{id}` | Update task        | `{title, description, status}` |
| `DELETE` | `/api/tasks/{id}` | Delete task        | —                              |

**Status values:** `"Pending"` · `"InProgress"` · `"Completed"`

---

## Troubleshooting

| Problem                                 | Cause                       | Fix                                             |
| --------------------------------------- | --------------------------- | ----------------------------------------------- |
| `dotnet not recognized`                 | PATH not set                | `$env:PATH += ";C:\Program Files\dotnet"`       |
| `The connection string '' is not valid` | `MONGODB_URI` not set       | Set it in the same terminal before `dotnet run` |
| `JWT secret is not configured`          | `JWT_SECRET` not set        | Add `$env:JWT_SECRET = "your-secret..."`        |
| `500` on Render                         | Render IPs blocked by Atlas | Atlas → Network Access → Add `0.0.0.0/0`        |
| Frontend "Failed to fetch"              | Wrong `VITE_API_URL`        | No trailing slash → redeploy frontend           |
| `401 Unauthorized`                      | Missing/expired token       | Log out and log back in                         |
| `git push` returns 403                  | Wrong GitHub account        | Use Personal Access Token in remote URL         |
| Render app slow to respond              | Free tier spin-down         | First request after ~15min sleep takes ~30s     |

---

## Tech Stack

| Layer            | Technology             |
| ---------------- | ---------------------- |
| Backend          | ASP.NET Core 8 (C#)    |
| Authentication   | JWT + BCrypt           |
| Database         | MongoDB Atlas          |
| Frontend         | React 18 + Vite        |
| Containerization | Docker (multi-stage)   |
| Hosting          | Render.com (free tier) |
