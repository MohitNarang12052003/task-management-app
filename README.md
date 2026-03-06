# TaskFlow — Full-Stack Task Management App

A production-ready Task Management app built with **ASP.NET Core 8** (backend API) + **React + Vite** (frontend) + **MongoDB Atlas** (database), deployable on **Render.com** for free.

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
│  (Vite Static Site) │◄──────►│  (Docker Web Service)│◄──────►│   (Free M0)  │
│  Render Static Site │        │  Render Web Service  │        │ cloud.mongodb│
└─────────────────────┘        └──────────────────────┘        └──────────────┘
```

### Project Structure

```
Render App Deployment/
├── render.yaml                        ← Render.com config
├── .gitignore
├── .env.example                       ← Env var reference
├── PREREQUISITES.md                   ← Setup requirements
├── README.md
├── TaskManagement.Api/                ← ASP.NET Core 8 API
│   ├── Controllers/TasksController.cs
│   ├── Services/                      ← Business logic
│   ├── Repositories/                  ← MongoDB data access
│   ├── Models/TaskItem.cs
│   ├── DTOs/                          ← Request/Response shapes
│   ├── Middleware/GlobalExceptionMiddleware.cs
│   ├── Program.cs
│   └── Dockerfile
└── Frontend/                          ← React + Vite
    ├── src/
    │   ├── App.jsx                    ← Main UI component
    │   ├── api.js                     ← API client
    │   └── index.css                  ← Dark theme styles
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
cd "Render App Deployment"
```

### 2. Run the Backend API

> **Windows — open PowerShell:**

```powershell
# If 'dotnet' is not recognized after installing .NET 8:
$env:PATH += ";C:\Program Files\dotnet"

# Set your MongoDB connection string
$env:MONGODB_URI = "mongodb+srv://USER:PASS@cluster0.xxxxx.mongodb.net/?retryWrites=true&w=majority"

# Navigate to the API project
cd TaskManagement.Api

# Restore packages and run
dotnet restore
dotnet run
```

API starts at: **http://localhost:8080**
Swagger UI (interactive docs): **http://localhost:8080** (opens automatically)

### 3. Run the Frontend

> **Open a second terminal:**

```powershell
cd Frontend
npm install
npm run dev
```

Frontend starts at: **http://localhost:5173**

### 4. Configure Frontend API URL

The `Frontend/.env` file tells the React app where the API is:

```env
VITE_API_URL=http://localhost:8080
```

> ⚠️ **Important:** This must be set to your **Render API URL** when deploying.

---

## Deploy to Render.com

### Step 1 — Push to GitHub

```powershell
cd "Render App Deployment"

git init
git add .
git commit -m "Initial commit"

# Create a repo at github.com first, then:
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

1. Go to [dashboard.render.com](https://dashboard.render.com) → **New + → Web Service**
2. Connect GitHub → select your repository
3. Configure:

   | Field          | Value                 |
   | -------------- | --------------------- |
   | Name           | `task-management-api` |
   | Root Directory | `TaskManagement.Api`  |
   | Runtime        | **Docker**            |
   | Instance Type  | Free                  |

4. Add Environment Variables:

   | Key                      | Value                                |
   | ------------------------ | ------------------------------------ |
   | `MONGODB_URI`            | Your MongoDB Atlas connection string |
   | `ASPNETCORE_ENVIRONMENT` | `Production`                         |

5. Click **Deploy Web Service** → wait ~5 minutes
6. ⭐ **Copy your API URL** (e.g. `https://task-management-api.onrender.com`)

---

### Step 3 — Deploy the Frontend

1. Render Dashboard → **New + → Static Site**
2. Connect same GitHub repo
3. Configure:

   | Field             | Value                      |
   | ----------------- | -------------------------- |
   | Name              | `task-management-frontend` |
   | Root Directory    | `Frontend`                 |
   | Build Command     | `npm run build`            |
   | Publish Directory | `dist`                     |

4. Add Environment Variable:

   | Key            | Value                                            |
   | -------------- | ------------------------------------------------ |
   | `VITE_API_URL` | Your API URL from Step 2 **(no trailing slash)** |

5. Click **Create Static Site** → wait ~2 minutes

---

### Step 4 — Enable CORS for the Frontend

Go to your **API service on Render** → Environment tab → Add:

| Key               | Value                                                                    |
| ----------------- | ------------------------------------------------------------------------ |
| `ALLOWED_ORIGINS` | Your frontend URL (e.g. `https://task-management-frontend.onrender.com`) |

Save → Render auto-redeploys the API.

---

### Step 5 — Verify Everything Works

Open your frontend URL — you should see the TaskFlow UI.

Test by creating a task. If it saves and appears in the list, your full stack is live! 🎉

---

## API Reference

Base URL: `http://localhost:8080` (local) or your Render URL (production)

| Method   | Endpoint          | Description    | Body                           |
| -------- | ----------------- | -------------- | ------------------------------ |
| `GET`    | `/api/tasks`      | Get all tasks  | —                              |
| `GET`    | `/api/tasks/{id}` | Get task by ID | —                              |
| `POST`   | `/api/tasks`      | Create task    | `{title, description, status}` |
| `PUT`    | `/api/tasks/{id}` | Update task    | `{title, description, status}` |
| `DELETE` | `/api/tasks/{id}` | Delete task    | —                              |

**Status values:** `"Pending"` · `"InProgress"` · `"Completed"`

**Example request:**

```json
POST /api/tasks
{
  "title": "Build login page",
  "description": "React form with validation",
  "status": "Pending"
}
```

**Example response:**

```json
{
  "id": "65f1a2b3c4d5e6f7a8b9c0d1",
  "title": "Build login page",
  "description": "React form with validation",
  "status": "Pending",
  "createdAt": "2026-03-06T07:00:00Z"
}
```

---

## Troubleshooting

| Problem                                 | Cause                       | Fix                                                                          |
| --------------------------------------- | --------------------------- | ---------------------------------------------------------------------------- |
| `dotnet not recognized`                 | PATH not updated            | Restart terminal or run `$env:PATH += ";C:\Program Files\dotnet"`            |
| `The connection string '' is not valid` | `MONGODB_URI` not set       | Set env var in same terminal before `dotnet run`. No newlines in the string! |
| `MongoAuthenticationException`          | Wrong credentials           | Re-check username/password in connection string                              |
| `500 Internal Server Error` on Render   | Render IPs blocked by Atlas | Atlas → Network Access → Add `0.0.0.0/0`                                     |
| Frontend shows "Failed to fetch"        | Wrong `VITE_API_URL`        | Check env var on Render → no trailing slash → redeploy frontend              |
| `git push` returns 403                  | Wrong GitHub account saved  | Use Personal Access Token in remote URL                                      |
| Render app slow first load              | Free tier spin-down         | Free tier sleeps after 15 min inactivity. First request takes ~30s           |

---

## Tech Stack

| Layer            | Technology           | Purpose              |
| ---------------- | -------------------- | -------------------- |
| Backend          | ASP.NET Core 8 (C#)  | REST API             |
| Database         | MongoDB Atlas        | Cloud NoSQL database |
| Frontend         | React 18 + Vite      | UI                   |
| Containerization | Docker (multi-stage) | Backend deployment   |
| Hosting          | Render.com           | Free cloud hosting   |
