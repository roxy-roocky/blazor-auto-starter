# AGENTS.md - Blazor Auto Starter

This file contains instructions and context for AI coding assistants to work effectively on this project.

## Project Overview

**Type**: Template/Starter for Blazor .NET 9 web applications
**Architecture**: Hybrid Auto render mode (Server-Side + WebAssembly)
**Database**: SQLite with Entity Framework Core
**Frontend**: Bootstrap 5, Font Awesome, SASS
**Features**: Authentication, Themes, Localization, Email system

### 7-Project Structure

1. **webapp** - Main server application (Blazor Server + WebAssembly hosting)
2. **webapp.Client** - Client-side WebAssembly application
3. **main-datacontext** - Shared data layer (EF Core, Identity)
4. **html-generator** - Email template rendering with Blazor components
5. **jobcode** - Business services (email senders, etc.)
6. **localization** - Localization system with resource management
7. **localization-generator** - CLI tool to generate .resx files from code

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
│   ├── Debug/        # Debug tools (EmailList, EmailTemplates)
│   ├── Layout/       # Layout components (NavBar, MainLayout)
│   └── Pages/        # Server pages (Home, Weather, etc.)
├── Managers/
│   ├── Account/      # Account services (EmailManager)
│   └── ThemeManager.cs
├── Styles/           # SASS sources (not published)
└── wwwroot/
    ├── js/           # JavaScript interop (ThemeSwitcher.js, CultureSwitcher.js)
    ├── bootstrap/    # Bootstrap compiled files
    ├── fontawesome/  # Font Awesome assets
    ├── logo.svg/png  # Application logo
    └── favicon.*     # Favicons and icons for all platforms

webapp.Client/
├── Components/
│   └── Layout/       # LanguageSwitcher component
├── Managers/
│   ├── ThemeManager.cs    # Client-side theme management
│   └── CultureManager.cs  # Client-side culture management
└── Pages/            # Client pages (Counter, Auth, etc.)

main-datacontext/
├── Identity/         # User models
├── Emails/           # Email data models
└── MainDbContext.cs  # EF Core context

html-generator/
├── Emails/
│   ├── Account/      # Account email templates (Confirmation, PasswordReset)
│   ├── Templates/    # Reusable templates (SimpleActionEmailTemplate)
│   └── Shared/       # Shared components (EmailLayout, TextBlock, ButtonBlock)
├── HtmlRendererService.cs  # Blazor to HTML renderer
└── EmailStyleConfig.cs     # Centralized email styling

jobcode/
└── Managers/
    └── Emails/       # IEmailSender, DbEmailSender, BrevoEmailSender

localization/
├── Configuration/
│   └── LocalizationConfig.cs  # Supported cultures configuration
├── Services/
│   └── SharedStringLocalizerFactory.cs  # Custom IStringLocalizer factory
└── Resources/        # .resx files (auto-generated)

localization-generator/
└── Program.cs        # CLI tool to scan code and generate .resx files
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

### Localization System

Multi-language support with automatic resource generation:
- **Configuration**: `localization/Configuration/LocalizationConfig.cs`
  - Define supported cultures (default: English, French)
  - Culture codes, display names, and flag emojis
- **Client Manager**: `webapp.Client/Managers/CultureManager.cs`
  - Detects browser language automatically
  - Persists user preference in cookies
- **JS Interop**: `webapp/wwwroot/js/CultureSwitcher.js`
- **UI Component**: `webapp.Client/Components/Layout/LanguageSwitcher.razor`

**Using localization in code**:
```csharp
// Inject IStringLocalizer
@inject IStringLocalizer<MyComponent> L

// Use translations
<h1>@L["PageTitle"]</h1>
<p>@L["WelcomeMessage", userName]</p>
```

**Generating resources**:
```bash
cd localization-generator
dotnet run ../webapp.sln  # Uses configured languages
dotnet run fr,en,es ../webapp.sln  # Custom languages
```

The generator:
- Scans all projects for `IStringLocalizer["Key"]` usage
- Extracts Data Annotations error messages
- Generates `.resx` files in `localization/Resources/`
- Preserves existing translations
- Moves obsolete files to `Obsolete/` folder

### Email System

Complete transactional email system with Blazor templates:
- **Templates**: Built with Blazor components in `html-generator/Emails/`
  - Rendered to HTML via `HtmlRendererService`
  - Reusable components: `EmailLayout`, `TextBlock`, `ButtonBlock`
  - Centralized styling in `EmailStyleConfig.cs`
- **Senders**: Two implementations (via `IEmailSender`)
  - `DbEmailSender`: Stores emails in DB for development
  - `BrevoEmailSender`: Sends via Brevo API for production
- **Debug Tools**:
  - `/Debug/EmailList`: View stored emails
  - `/Debug/EmailTemplates`: Preview all templates

**Using email system**:
```csharp
// Inject IEmailSender
private readonly IEmailSender _emailSender;

// Send email
await _emailSender.SendEmailAsync(
    toEmail: "user@example.com",
    subject: "Subject",
    htmlMessage: htmlContent
);
```

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
- Brevo API keys

These files are already in `.gitignore`.

### Authentication

- ASP.NET Core Identity configured and ready
- User model: `main_datacontext.Identity.User`
- Email sender: Uses `IEmailSender` interface
  - Development: Emails stored in database
  - Production: Sent via Brevo API (requires API key in `appsettings.json`)
- Account confirmation emails use Blazor templates
- Password reset with styled email templates

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

1. **Configure Brevo API** - Add your Brevo API key to `appsettings.json`
   ```json
   {
     "Brevo": {
       "ApiKey": "your-api-key-here"
     }
   }
   ```
2. **Configure** real database (PostgreSQL/SQL Server recommended for production)
3. **Enable** HTTPS in production (already configured in `Program.cs`)
4. **Check** User Secrets for sensitive data
5. **Generate localization files** - Run `localization-generator` before deployment
6. **Compile SASS** - Ensure all SASS files are compiled to CSS

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
- Examples (Counter, Weather, LocalizationExample) illustrate different features
- Always **test** EF migrations before applying them
- When unsure about render mode, ask user about their use case

### Key Features to Remember

1. **Localization**: Always use `IStringLocalizer<T>` for user-facing strings
   - Run `localization-generator` after adding new translatable strings
   - Update `LocalizationConfig.cs` to add new languages

2. **Email Templates**: Use Blazor components in `html-generator/Emails/`
   - Follow existing patterns (EmailLayout, TextBlock, ButtonBlock)
   - Test templates via `/Debug/EmailTemplates` page
   - Emails are stored in DB during development

3. **Theme System**: Client-side management via cookies
   - Use Bootstrap's `data-bs-theme` attribute
   - Don't create custom theme-switching logic

4. **Icons & Branding**: Logo and favicons are in `wwwroot/`
   - `logo.svg/png` for application branding
   - Multiple favicon formats for cross-platform support

5. **Debug Tools**: Available to authenticated users
   - `/Debug/EmailList` - View development emails
   - `/Debug/EmailTemplates` - Preview email templates
