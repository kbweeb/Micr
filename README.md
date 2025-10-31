# Micr

A 4-layer .NET 8 solution:
- Domain (POCOs only)
- DataAccessLogic (EF Core + repositories + AutoMapper)
- BusinessLogic (services + validation)
- MicrUI (ASP.NET Core web app — the ONLY runnable project)

This guide explains how to clone, configure, build, and run the app on any Windows machine.

## 1) Prerequisites
- Windows 10/11
- .NET SDK 8.0: https://dotnet.microsoft.com/download/dotnet/8.0
- SQL Server (Express is fine). Examples use a local named instance like SQLEXPRESS.
  - SQL Server Management Studio (SSMS) recommended

## 2) Clone and choose the branch
`powershell
# Clone
git clone https://github.com/kbweeb/Micr.git
cd Micr

# Until the PR is merged, use the scaffold branch
git fetch origin
git checkout feat/nlayer-scaffold
`

## 3) Restore and build
`powershell
# Restore all projects
dotnet restore

# Build the whole solution
dotnet build -c Debug
`

## 4) Configure database connection
MicrUI runs against SQL Server by default. Update your connection string to point to your local SQL Server instance.

1. Open MicrUI/appsettings.Development.json (or ppsettings.json if you prefer) and set the connection string. Examples:
   - Windows auth (trusted connection):
     `json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MicrDb;Trusted_Connection=True;TrustServerCertificate=True"
       }
     }
     `
   - SQL auth:
     `json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MicrDb;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True"
       }
     }
     `

2. If the project has a UseInMemory flag, set it to alse to use SQL Server.

Notes:
- Ensure the SQLEXPRESS instance (or your chosen instance) exists and is running.
- TrustServerCertificate=True helps on dev machines without a trusted cert.

## 5) Apply or create EF Core migrations
If MicrUI is configured to auto-migrate on startup, you can skip manual commands. Otherwise, use:
`powershell
# From the solution root
# Create a migration (only if you changed models)
# dotnet ef migrations add Init --project MicrUI --startup-project MicrUI

# Apply migrations to the configured database
# dotnet ef database update --project MicrUI --startup-project MicrUI
`
If you don’t have the EF CLI installed:
`powershell
dotnet tool update --global dotnet-ef
`

## 6) Run the application
Run the web app (MicrUI is the only executable):
`powershell
# Option A: CLI
dotnet run --project MicrUI -c Debug

# Option B: Visual Studio
# - Open Micr.sln
# - Right-click MicrUI -> Set as Startup Project
# - Press F5
`
The app will start and listen on the URL shown in the console (typically https://localhost:5xxx or http://localhost:5xxx).

## 7) How the layers are wired
- MicrUI depends on BusinessLogic and DataAccessLogic.
- BusinessLogic depends on DataAccessLogic and Domain.
- DataAccessLogic depends on Domain.
- Domain has no dependencies.

Dependency Injection:
- Data access registrations live in DataAccessLogic/DataAccessRegistration.cs.
- Business services live under BusinessLogic/Logic and validators in BusinessLogic/Validation.

If not already added, wire up registrations in MicrUI/Program.cs:
`csharp
// using DataAccessLogic;
// using BusinessLogic;

// builder.Services.AddDataAccessLogic(builder.Configuration);
// builder.Services.AddBusinessLogic();
`

## 8) Troubleshooting
- SQL Server not found: verify instance name (e.g., localhost\\SQLEXPRESS) and that the SQL Server service is running.
- Login failed: if using SQL auth, confirm the user/password and that SQL Server allows mixed-mode authentication.
- Cannot connect due to certificate/trust issues: add TrustServerCertificate=True to your connection string.
- Port already in use: stop other apps using the same port or let Kestrel choose a free one.
- EF CLI not found: run dotnet tool update --global dotnet-ef.

## 9) Recommended developer workflow
- Create feature branches from main.
- Keep only MicrUI runnable; class libraries should compile but not run directly.
- Commit small, logical changes and open PRs.

---
If you need me to pre-wire DI in MicrUI or provide sample seed data, say “wire it” and I’ll add it.
