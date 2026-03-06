# Prerequisites

Everything you need before building and deploying this project.

---

## 1. Accounts (Free)

| Service           | Purpose                   | Sign Up                                        |
| ----------------- | ------------------------- | ---------------------------------------------- |
| **MongoDB Atlas** | Cloud database            | [cloud.mongodb.com](https://cloud.mongodb.com) |
| **GitHub**        | Host your code            | [github.com](https://github.com)               |
| **Render.com**    | Deploy backend + frontend | [render.com](https://render.com)               |

---

## 2. Software to Install

### .NET 8 SDK

- Download: [dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- Choose: **SDK 8.0.x (x64)** for your OS
- Verify (after restarting terminal):
  ```powershell
  dotnet --version   # should print 8.0.x
  ```
- **Windows only:** If `dotnet` is not recognized, add to PATH:
  ```powershell
  $env:PATH += ";C:\Program Files\dotnet"
  ```

### Node.js & npm

- Download: [nodejs.org](https://nodejs.org) → LTS version
- Verify:
  ```bash
  node --version   # e.g. v20.x.x
  npm --version    # e.g. 10.x.x
  ```

### Git

- Download: [git-scm.com/download](https://git-scm.com/download)
- Verify:
  ```bash
  git --version
  ```

---

## 3. Environment Variables

Three environment variables are required to run the backend. All must be set in the **same terminal session** before `dotnet run`.

| Variable                 | Description                                      | Example                                             |
| ------------------------ | ------------------------------------------------ | --------------------------------------------------- |
| `MONGODB_URI`            | MongoDB Atlas connection string                  | `mongodb+srv://user:pass@cluster0.xxx.mongodb.net/` |
| `JWT_SECRET`             | Secret key for signing JWT tokens (min 32 chars) | `my-super-secret-key-min-32-chars!!`                |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment                              | `Development` or `Production`                       |

**Windows PowerShell:**

```powershell
$env:MONGODB_URI  = "your-connection-string"
$env:JWT_SECRET   = "your-secret-key-at-least-32-characters!"
```

> ⚠️ `JWT_SECRET` must be at least 32 characters. Use any long random string.

---

## 4. MongoDB Atlas Setup

1. Sign up at [cloud.mongodb.com](https://cloud.mongodb.com)
2. Create a **free cluster** (M0 — Free Forever)
3. Create a **database user**:
   - Security → Database Access → Add New Database User
   - Choose: **Password authentication**
   - Note down **username** and **password**
4. Get your **connection string**:
   - Clusters → Connect → Drivers → C# / .NET → 2.25 or later
   - Toggle **Show Password ON**
   - Copy the string (starts with `mongodb+srv://...`)
5. Allow Render's IPs:
   - Security → Network Access → Add IP Address
   - Click **"Allow Access from Anywhere"** → `0.0.0.0/0` → Confirm

---

## 5. GitHub Setup

1. Create an account at [github.com](https://github.com)
2. Create a **new repository** (empty — no README, no .gitignore)
3. Keep the repo URL handy: `https://github.com/YOUR_USERNAME/YOUR_REPO`

---

## 6. Render.com Setup

1. Sign up at [render.com](https://render.com)
2. Connect your GitHub account (click "Authorize Render")
3. No credit card required for free tier

---

## 7. Render Environment Variables

When deploying on Render, add these to the **API Web Service** → Environment tab:

| Key                      | Value                                       |
| ------------------------ | ------------------------------------------- |
| `MONGODB_URI`            | Your MongoDB Atlas connection string        |
| `JWT_SECRET`             | Same secret key used locally (min 32 chars) |
| `ASPNETCORE_ENVIRONMENT` | `Production`                                |
| `ALLOWED_ORIGINS`        | Your frontend Render URL (for CORS)         |

---

## 8. Windows Credential Note

If you have **multiple GitHub accounts** on the same machine, Git may use the wrong one. Fix with a Personal Access Token:

1. GitHub → Settings → Developer Settings → Personal Access Tokens → Tokens (classic)
2. Generate token with **`repo`** scope
3. Update your remote URL:
   ```powershell
   git remote set-url origin https://YOUR_USERNAME:YOUR_TOKEN@github.com/YOUR_USERNAME/YOUR_REPO.git
   ```
