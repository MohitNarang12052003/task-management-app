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

## 2. Software to Install Locally

### .NET 8 SDK

- Download: [dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- Choose: **SDK 8.0.x (x64)** for your OS
- After install, **restart your terminal**, then verify:
  ```powershell
  dotnet --version   # should print 8.0.x
  ```
- **Windows only:** If `dotnet` is not recognized after restart, add it to PATH:
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

## 3. MongoDB Atlas Setup

1. Sign up at [cloud.mongodb.com](https://cloud.mongodb.com)
2. Create a **free cluster** (M0 — Free Forever)
3. Create a **database user**:
   - Security → Database Access → Add New Database User
   - Choose: **Password authentication**
   - Note down the **username** and **password**
4. Get your **connection string**:
   - Clusters → Connect → Drivers → C# / .NET → 2.25 or later
   - Toggle **Show Password ON**
   - Copy the string (starts with `mongodb+srv://...`)
5. Allow network access:
   - Security → Network Access → Add IP Address
   - Click **"Allow Access from Anywhere"** → `0.0.0.0/0` → Confirm

---

## 4. GitHub Setup

1. Create an account at [github.com](https://github.com)
2. Create a **new repository** (empty — no README, no .gitignore)
3. Keep the repo URL handy: `https://github.com/YOUR_USERNAME/YOUR_REPO`

---

## 5. Render.com Setup

1. Sign up at [render.com](https://render.com)
2. Connect your GitHub account when prompted (click "Authorize Render")
3. No credit card required for free tier

---

## 6. Windows Credential Note

If you have **multiple GitHub accounts** on the same machine, Git may use the wrong one. Use a Personal Access Token to avoid issues:

1. GitHub → Settings → Developer Settings → Personal Access Tokens → Tokens (classic)
2. Generate a token with **`repo`** scope
3. Use it in your push URL:
   ```powershell
   git remote set-url origin https://YOUR_USERNAME:YOUR_TOKEN@github.com/YOUR_USERNAME/YOUR_REPO.git
   ```
