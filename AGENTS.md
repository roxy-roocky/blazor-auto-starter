# AGENTS.md - Blazor Auto Starter

This file contains instructions and context for AI coding assistants to work effectively on this project.

## Project Overview

**Type**: Template/Starter for Blazor .NET 9 web applications
**Architecture**: Hybrid Auto render mode (Server-Side + WebAssembly)
**Database**: SQLite with Entity Framework Core
**Frontend**: Bootstrap 5, Font Awesome, SASS

### 3-Project Structure

1. **webapp** - Main server application (Blazor Server + WebAssembly hosting)
2. **webapp.Client** - Client-side WebAssembly application
3. **main-datacontext** - Shared data layer (EF Core, Identity)

## Initial Setup

### Installing Dependencies

```bash
# npm dependencies (from webapp/)
npm install

# NuGet packages (from root)
dotnet restore
```

### Configuration Files

`appsettings.json` files are NOT versioned. Always create from examples:

```bash
# Server
cp webapp/appsettings.example.json webapp/appsettings.json

# Client
cp webapp.Client/wwwroot/appsettings.example.json webapp.Client/wwwroot/appsettings.json
```

### Database

```bash
# Create a migration (from webapp/)
dotnet ef migrations add MigrationName

# Apply to DB
dotnet ef database update
```

## Build & Test

### Build Commands

```bash
# Development with hot-reload (from webapp/)
dotnet watch run

# Production build
dotnet build -c Release

# Publish
dotnet publish -c Release
```

### Asset Compilation

Build automatically compiles:
- SASS → CSS (Styles/ → wwwroot/)
- Bootstrap JS (copied from node_modules/)
- Font Awesome (SASS + webfonts)

For manual SASS development:
```bash
npx sass -q Styles/:wwwroot/ --watch
```

## Architecture & Conventions

### Blazor Render Modes

**Important**: This project uses **Auto** render mode which combines Server and WebAssembly.

- **Server-side pages**: Use `@attribute [StreamRendering]`
  - Example: `webapp/Components/Pages/Weather.razor`
  - For dynamic data, reduces client load

- **Client-side pages**: Use `@rendermode InteractiveWebAssembly`
  - Example: `webapp.Client/Pages/Counter.razor`
  - For rich interactions, offline functionality

### File Organization

```
webapp/
├── Components/
│   ├── Account/      # Authentication and account management
│   ├── Layout/       # Layout components
│   └── Pages/        # Server pages
├── Managers/         # Server services (e.g., ThemeManager)
├── Styles/           # SASS sources (not published)
└── wwwroot/          # Compiled public assets

webapp.Client/
├── Components/       # WebAssembly components
├── Managers/         # Client services (ThemeManager)
└── Pages/            # Client pages (Counter, etc.)

main-datacontext/
├── Identity/         # User models
└── MainDbContext.cs  # EF Core context
```

### Naming Conventions

- **Razor Components**: PascalCase with `.razor` extension
- **Services/Managers**: `Manager` suffix (e.g., `ThemeManager`)
- **EF Migrations**: Descriptive PascalCase (e.g., `AddUserPreferences`)

### Theme Management

Theme system is managed **client-side**:
- **C# Manager**: `webapp.Client/Managers/ThemeManager.cs`
- **JS Interop**: `webapp/wwwroot/js/ThemeSwitcher.js`
- **Persistence**: Cookies via JavaScript
- **Bootstrap Attribute**: `data-bs-theme="light|dark"`

## Code Standards

### C# / Razor

- Use `ImplicitUsings` (enabled in all projects)
- Nullable enabled: always type `?` for nullable values
- Prefer functional components over class-based when possible
- Inject services via `@inject` at top of Razor components

### Styles

- **SASS only** in `Styles/`, never direct CSS
- Don't edit files in `wwwroot/` (auto-generated)
- Use existing Bootstrap variables and mixins
- Bootstrap classes are preferred over custom styles

### JavaScript

- Minimal: only for necessary interop
- Namespace pattern: `window.ModuleName = { ... }`
- Example: `ThemeSwitcher.js` exposes `window.ThemeSwitcher`

## Security

### Sensitive Data

**NEVER version**:
- `appsettings.json` (server)
- `appsettings.Development.json` (server)
- `wwwroot/appsettings.json` (client)
- `.db` files (SQLite database)
- `Data/` folder (contains DB)

These files are already in `.gitignore`.

### Authentication

- ASP.NET Core Identity configured and ready
- User model: `main_datacontext.Identity.User`
- Email sender: `IdentityNoOpEmailSender` (replace in production)
- Account confirmation required by default

## Git Workflow

### Commits

No strict convention, but keep descriptive and concise.

### Files to Check Before Commit

- Don't commit generated files in `wwwroot/`
- Verify `.gitignore` is respected
- Ensure EF migrations are consistent

## VSCode Tasks

Project includes preconfigured tasks (`.vscode/tasks.json`):

**Daily Development**:
- `💄 Watch SASS` - Auto-compile SASS
- `⌚ Watch Web` - Hot-reload the app

**Entity Framework**:
- `🗃️ EF - ➕ Ajouter une migration` (Add migration)
- `🗃️ EF - ⬆️ Mettre à jour la base de données` (Update database)
- `🗃️ EF - 📑 Lister les migrations` (List migrations)

Access: `Ctrl+Shift+P` → "Run Task"

## Patterns to Follow

### ✅ Good

```razor
@* Client-side page with explicit rendermode *@
@page "/counter"
@rendermode InteractiveWebAssembly

@code {
    private int count = 0;
}
```

```csharp
// Clean dependency injection
@inject ThemeManager ThemeManager

// Proper nullable usage
public User? CurrentUser { get; set; }
```

### ❌ Avoid

```razor
@* Don't mix render modes without reason *@
@* Don't omit @rendermode on client pages *@
```

```csharp
// Avoid null without explicit nullable
public User CurrentUser { get; set; } // Bad if can be null
```

## Deployment

### Important Points

1. **Replace** `IdentityNoOpEmailSender` with real email service
2. **Configure** real database (PostgreSQL/SQL Server recommended)
3. **Enable** HTTPS in production (already configured in `Program.cs`)
4. **Check** User Secrets for sensitive data

### Production Build

```bash
dotnet publish -c Release -o ./publish
```

The `publish/` folder will contain the ready-to-deploy application.

## External Resources

- [Blazor .NET 9 Documentation](https://learn.microsoft.com/aspnet/core/blazor/)
- [Bootstrap 5.3](https://getbootstrap.com/docs/5.3/)
- [Font Awesome 7](https://fontawesome.com/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)

## Notes for AI Agents

- This project is a **template/starter**, designed to be cloned and modified
- Prioritize **simplicity** and **code clarity**
- Examples (Counter, Weather) are there to **illustrate** both render modes
- Always **test** EF migrations before applying them
- When unsure about render mode, ask user about their use case
