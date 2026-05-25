# Blazor Auto Starter

Template de démarrage moderne pour application web Blazor .NET 10 avec mode de rendu automatique (Auto), combinant Server-Side Rendering (SSR) et WebAssembly pour des performances optimales.

## Table des matières

- [Fonctionnalités](#fonctionnalités)
- [Prérequis](#prérequis)
- [Initialisation du projet](#initialisation-du-projet)
  - [1. Cloner le dépôt](#1-cloner-le-dépôt)
  - [2. Installer les dépendances npm](#2-installer-les-dépendances-npm)
  - [3. Configurer les fichiers appsettings](#3-configurer-les-fichiers-appsettings)
  - [4. Restaurer les packages NuGet](#4-restaurer-les-packages-nuget)
  - [5. Créer et appliquer les migrations de base de données](#5-créer-et-appliquer-les-migrations-de-base-de-données)
  - [6. Lancer l'application](#6-lancer-lapplication)
  - [6. Développement avec hot-reload](#6-développement-avec-hot-reload)
- [Structure du projet](#structure-du-projet)
- [Technologies utilisées](#technologies-utilisées)
- [Exemples de rendu](#exemples-de-rendu)
  - [📱 Page Counter - Rendu côté client (WebAssembly)](#-page-counter---rendu-côté-client-webassembly)
  - [🌤️ Page Weather - Rendu côté serveur (Server-Side)](#️-page-weather---rendu-côté-serveur-server-side)
  - [📧 Page Email Templates - Prévisualisation des templates](#-page-email-templates---prévisualisation-des-templates)
- [Configuration](#configuration)
  - [Fichiers appsettings](#fichiers-appsettings)
  - [Thèmes](#thèmes)
  - [Localisation](#localisation)
  - [Icônes et favicons](#icônes-et-favicons)
  - [Emails](#emails)
- [Développement](#développement)
  - [Extensions VSCode recommandées](#extensions-vscode-recommandées)
  - [Tâches VSCode disponibles](#tâches-vscode-disponibles)
  - [Commandes en ligne](#commandes-en-ligne)
- [Licence](#licence)

## Fonctionnalités

- 🔐 **Authentification complète** - Système d'identité ASP.NET Core avec gestion des utilisateurs
- 🎨 **Thèmes personnalisables** - Basculement dynamique entre thème clair et sombre avec persistance par cookies
- 🌍 **Système de localisation** - Support multilingue avec générateur automatique de ressources .resx et sélecteur de langue
- ⚡ **Rendu hybride** - Mode Auto combinant rendu serveur (InteractiveServer) et WebAssembly (InteractiveWebAssembly)
- 💾 **Base de données SQLite** - Entity Framework Core avec migrations pour la gestion des données
- 📧 **Système d'envoi d'emails** - Templates d'emails personnalisables avec composants Blazor et intégration Brevo API
- 🎭 **Bootstrap 5** - Interface utilisateur responsive avec intégration Bootstrap 5.3
- ✨ **Font Awesome** - Bibliothèque d'icônes complète (v7.0)
- 🔧 **SASS** - Compilation automatique des styles SASS
- 📋 **Serilog** - Logging structuré avec sortie console et fichiers journaux rotatifs
- 🏗️ **Architecture modulaire** - Séparation claire entre client, serveur et couche de données

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (pour la compilation SASS et les dépendances frontend)

## Initialisation du projet

### 1. Cloner le dépôt

```bash
git clone <url-du-depot>
cd blazor-auto-starter
```

### 2. Installer les dépendances npm

```bash
cd webapp
npm install
```

Cette commande installe :
- Bootstrap 5.3.8
- Font Awesome 7.0.1
- SASS 1.92.1

### 3. Configurer les fichiers appsettings

Le projet utilise des fichiers de configuration d'exemple pour protéger les données sensibles :

**Pour le projet serveur :**
```bash
cd webapp
cp appsettings.example.json appsettings.json
```

**Pour le projet client :**
```bash
cd ../webapp.Client/wwwroot
cp appsettings.example.json appsettings.json
```

Éditez ensuite ces fichiers selon vos besoins (chaînes de connexion, clé API Brevo, etc.). Pour utiliser l'envoi d'emails en production, ajoutez votre clé API Brevo dans `webapp/appsettings.json` :

```json
{
  "Brevo": {
    "ApiKey": "votre-clé-api-brevo"
  }
}
```

> **Note :** En développement, les emails sont stockés dans la base de données (table `Emails`). En production, ils sont envoyés via l'API Brevo.

### 4. Restaurer les packages NuGet

```bash
cd ../..
dotnet restore
```

### 5. Créer et appliquer les migrations de base de données

```bash
cd webapp
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 6. Lancer l'application

```bash
dotnet run
```

L'application sera accessible à l'adresse `https://localhost:5001` (ou le port configuré).

### 6. Développement avec hot-reload

Pour le développement avec rechargement automatique :

```bash
dotnet watch run
```

Les fichiers `.js` et `.css` sont surveillés automatiquement grâce à la configuration du fichier [webapp.csproj](webapp/webapp.csproj).

## Structure du projet

### 📁 Solution principale (`webapp.sln`)

Le projet est organisé en sept projets distincts :

#### **webapp** - Application serveur principale
- **Components/** - Composants Razor partagés
  - **Account/** - Composants d'authentification et gestion de compte
  - **Debug/** - Outils de débogage (EmailList, EmailTemplates)
  - **Layout/** - Composants de mise en page (navigation, footer, etc.)
  - **Pages/** - Pages principales (Home, Weather, Error)
- **Managers/** - Services et gestionnaires
  - **Account/** - Gestionnaires de compte (EmailManager pour les emails transactionnels)
- **wwwroot/** - Ressources statiques
  - **js/** - Scripts JavaScript (ThemeSwitcher.js, CultureSwitcher.js)
  - **bootstrap/** - Fichiers Bootstrap compilés
  - **fontawesome/** - Styles et webfonts Font Awesome
  - **logo.svg/png** - Logo de l'application
  - **favicon.*** - Favicons et icônes pour différentes plateformes (16x16, 32x32, Android, Apple)
- **Styles/** - Fichiers sources SASS
- **Program.cs** - Point d'entrée et configuration des services

#### **webapp.Client** - Application WebAssembly
- **Components/** - Composants côté client
  - **Layout/** - Composants de mise en page (LanguageSwitcher)
- **Managers/** - Gestionnaires côté client
  - **CultureManager.cs** - Gestion de la culture et du changement de langue
- **Pages/** - Pages rendues côté client
- **wwwroot/** - Ressources statiques spécifiques au client
- **Program.cs** - Configuration WebAssembly

#### **main-datacontext** - Couche de données
- **Emails/** - Modèles de données pour les emails
- Contexte Entity Framework Core
- Modèles de données
- Configuration Identity
- Gestion des migrations

#### **html-generator** - Génération de templates HTML
- **HtmlRendererService.cs** - Service de rendu de composants Blazor en HTML
- **Emails/** - Templates d'emails réutilisables
  - **Account/** - Templates pour le système de compte (Confirmation, PasswordReset, PasswordResetLink)
  - **Templates/** - Composants de templates complets (SimpleActionEmailTemplate, etc.)
  - **Shared/** - Composants partagés (EmailLayout, TextBlock, ButtonBlock)
  - **EmailStyleConfig.cs** - Configuration centralisée des couleurs et styles des emails
- Compatible avec tous les clients email grâce aux tables HTML

#### **jobcode** - Services métier
- **Managers/Emails/** - Gestionnaires d'envoi d'emails
  - **IEmailSender** - Interface d'envoi d'emails
  - **DbEmailSender** - Stockage des emails en base de données (développement)
  - **BrevoEmailSender** - Envoi via API Brevo (production)

#### **localization** - Système de localisation
- **Configuration/LocalizationConfig.cs** - Configuration centralisée des cultures supportées
- **Services/SharedStringLocalizerFactory.cs** - Factory personnalisée pour IStringLocalizer
- **Resources/** - Fichiers de ressources .resx générés automatiquement

#### **localization-generator** - Générateur de ressources
- **Program.cs** - Outil CLI pour scanner le code et générer les fichiers .resx
- Analyse automatique des fichiers C# et Razor
- Extraction des clés IStringLocalizer et Data Annotations
- Gestion intelligente des ressources obsolètes

### 🔄 Flux de compilation

1. **SASS** : Les fichiers `.scss` dans `Styles/` sont compilés vers `wwwroot/`
2. **Bootstrap** : Les fichiers JavaScript sont copiés depuis `node_modules/`
3. **Font Awesome** : SASS compilé et webfonts copiés automatiquement
4. **Blazor** : Compilation des composants Razor en C#

## Technologies utilisées

- **Framework** : .NET 10
- **UI** : Blazor (mode Auto - Server + WebAssembly)
- **Base de données** : SQLite avec Entity Framework Core 10
- **Authentification** : ASP.NET Core Identity
- **Logging** : Serilog (console + fichiers journaux rotatifs)
- **CSS** : Bootstrap 5.3 + SASS
- **Icônes** : Font Awesome 7.0

## Exemples de rendu

Le template inclut des exemples démontrant les différents modes de rendu :

### 📱 Page Counter - Rendu côté client (WebAssembly)

La page [Counter](webapp.Client/Pages/Counter.razor) (`/counter`) utilise le mode `@rendermode InteractiveWebAssembly`. Cette page :
- S'exécute entièrement dans le navigateur
- Continue de fonctionner même sans connexion au serveur
- Offre une réactivité instantanée sans round-trip serveur
- Idéale pour les interactions utilisateur intensives

### 🌤️ Page Weather - Rendu côté serveur (Server-Side)

La page [Weather](webapp/Components/Pages/Weather.razor) (`/weather`) utilise le mode `[StreamRendering]`. Cette page :
- S'exécute côté serveur avec streaming du contenu
- Permet un chargement progressif de l'interface
- Réduit la charge côté client
- Idéale pour l'affichage de données dynamiques

### 📧 Outils de débogage Email

Le menu Debug (accessible aux utilisateurs authentifiés) contient des outils pour tester les emails :

**Page Email Templates** (`/Debug/EmailTemplates`) :
- Prévisualise tous les templates d'emails disponibles
- Affiche les templates dans un accordéon interactif avec iframe
- Démontre l'utilisation des composants `EmailLayout`, `TextBlock` et `ButtonBlock`

**Page Email List** (`/Debug/EmailList`) :
- Affiche tous les emails stockés en base de données (mode développement)
- Permet de visualiser le contenu HTML des emails envoyés
- Utile pour tester le système d'emails sans configuration SMTP

## Configuration

### Fichiers appsettings

Les fichiers de configuration doivent être créés à partir des fichiers d'exemple :
- Serveur : [appsettings.example.json](webapp/appsettings.example.json) → `appsettings.json`
- Client : [appsettings.example.json](webapp.Client/wwwroot/appsettings.example.json) → `appsettings.json`

La chaîne de connexion SQLite par défaut pointe vers `../Data/app.db` avec cache partagé.

### Thèmes

Le système de thèmes est géré **côté client** via le [ThemeManager](webapp.Client/Managers/ThemeManager.cs) et utilise :
- Les attributs `data-bs-theme` de Bootstrap pour le basculement clair/sombre
- La persistance des préférences utilisateur via cookies
- Le script [ThemeSwitcher.js](webapp/wwwroot/js/ThemeSwitcher.js) pour l'interaction JavaScript

### Localisation

Le projet intègre un système complet de localisation multilingue :

**Configuration :**
- **Cultures supportées** - Définies dans [LocalizationConfig.cs](localization/Configuration/LocalizationConfig.cs) (anglais et français par défaut)
- **Culture par défaut** - Anglais (configurable)
- **Détection automatique** - Utilise la langue du navigateur si disponible, sinon la culture par défaut
- **Persistance** - Les préférences utilisateur sont stockées dans des cookies

**Composants :**
- **LanguageSwitcher** - Composant de sélection de langue avec drapeaux et noms de langues ([LanguageSwitcher.razor](webapp.Client/Components/Layout/LanguageSwitcher.razor))
- **CultureManager** - Gestionnaire côté client pour le changement de culture ([CultureManager.cs](webapp.Client/Managers/CultureManager.cs))
- **CultureSwitcher.js** - Script JavaScript pour la gestion des cookies de culture ([CultureSwitcher.js](webapp/wwwroot/js/CultureSwitcher.js))

**Générateur de ressources :**
Le projet inclut un outil CLI [localization-generator](localization-generator/) qui :
- Scanne automatiquement tous les projets pour trouver les clés de traduction
- Extrait les utilisations de `IStringLocalizer["Key"]` dans les fichiers C# et Razor
- Extrait les messages d'erreur des Data Annotations (`[Required(ErrorMessage = "...")]`)
- Génère des fichiers `.resx` pour chaque langue dans `localization/Resources/`
- Conserve les traductions existantes lors de la régénération
- Déplace automatiquement les fichiers obsolètes dans un dossier `Obsolete/`

**Utilisation dans le code :**
```csharp
// Dans un fichier Razor
@inject IStringLocalizer<LocalizationExample> L
<h1>@L["PageTitle"]</h1>
<p>@L["WelcomeMessage", userName]</p>

// Dans une classe C#
private readonly IStringLocalizer<MyClass> _localizer;
var message = _localizer["Key"];
```

**Commandes :**
```bash
# Générer les fichiers de ressources pour toutes les langues configurées
cd localization-generator
dotnet run ../webapp.sln

# Générer pour des langues spécifiques
dotnet run fr,en,es ../webapp.sln
```

Exemple de page : [LocalizationExample.razor](webapp/Components/Pages/LocalizationExample.razor)

### Icônes et favicons

Le projet inclut un ensemble complet d'icônes et de favicons pour différentes plateformes :

**Fichiers disponibles :**
- **logo.svg/png** - Logo principal de l'application (Blazor Auto Starter avec icône stylisée)
- **favicon.svg/png** - Favicon moderne pour les navigateurs récents
- **favicon-16x16.png, favicon-32x32.png** - Favicons en différentes tailles
- **apple-touch-icon.png** - Icône pour iOS (180x180)
- **android-chrome-192x192.png, android-chrome-512x512.png** - Icônes pour Android PWA
- **site.webmanifest** - Manifest web pour les Progressive Web Apps

Le logo utilise un design moderne avec :
- Un cercle avec dégradé brun (couleurs Blazor)
- Une lettre "B" stylisée en blanc
- Un accent de fusée avec dégradé orange/jaune symbolisant la performance

Tous les fichiers sont dans [webapp/wwwroot/](webapp/wwwroot/).

### Logging

Le projet utilise [Serilog](https://serilog.net/) pour le logging structuré, configuré via `appsettings.json` :

**Packages installés :**
- `Serilog.AspNetCore` - Intégration avec ASP.NET Core
- `Serilog.Settings.Configuration` - Configuration via `appsettings.json`
- `Serilog.Sinks.Console` - Sortie console formatée
- `Serilog.Sinks.File` - Fichiers journaux rotatifs

**Configuration dans `appsettings.json` :**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

Les fichiers de logs sont générés dans le dossier `logs/` avec rotation journalière.

### Emails

Le projet intègre un système complet d'envoi d'emails transactionnels avec templates Blazor :

**Architecture :**
- **Templates Blazor** - Les emails sont construits avec des composants Blazor dans [html-generator/Emails/](html-generator/Emails/), rendus en HTML via `HtmlRendererService`
- **Composants réutilisables** - `EmailLayout`, `TextBlock`, `ButtonBlock` pour créer des emails responsives compatibles tous clients
- **Configuration centralisée** - Couleurs et styles définis dans [EmailStyleConfig.cs](html-generator/Emails/EmailStyleConfig.cs)

**Modes d'envoi :**
- **Développement** - Les emails sont stockés en base de données (table `Emails`) via `DbEmailSender`, consultables dans `/Debug/EmailList`
- **Production** - Envoi via l'API Brevo avec `BrevoEmailSender`, nécessite une clé API dans `appsettings.json`

**Templates disponibles :**
- Confirmation d'email
- Réinitialisation de mot de passe
- Template générique `SimpleActionEmailTemplate` pour les actions utilisateur

**Utilisation :**
```csharp
// Injection du service
private readonly IEmailSender _emailSender;

// Envoi d'un email
await _emailSender.SendEmailAsync(
    toEmail: "user@example.com",
    subject: "Sujet",
    htmlMessage: htmlContent
);
```

## Développement

### Extensions VSCode recommandées

Le projet inclut une configuration VSCode avec des extensions recommandées dans [.vscode/extensions.json](.vscode/extensions.json) :

- **Bootstrap IntelliSense** (`hossaini.bootstrap-intellisense`) - Autocomplétion pour les classes Bootstrap
- **Task Explorer** (`spmeesseman.vscode-taskexplorer`) - Interface graphique pour gérer les tâches VSCode
- **C# Dev Kit** (`ms-dotnettools.csdevkit`) - Outils de développement C# et .NET

VSCode vous proposera automatiquement d'installer ces extensions à l'ouverture du projet.

### Tâches VSCode disponibles

Le fichier [.vscode/tasks.json](.vscode/tasks.json) contient plusieurs tâches préconfigurées accessibles via `Ctrl+Shift+P` > "Run Task" :

**Développement :**
- **💄 Watch SASS** - Compilation automatique des fichiers SASS en mode watch
- **⌚ Watch Web** - Lancement de l'application avec hot-reload
- **⚙️ Build Web** - Build du projet web

**Gestion Entity Framework :**
- **🗃️ EF - ➕ Ajouter une migration** - Créer une nouvelle migration (demande le nom)
- **🗃️ EF - 🚫 Supprimer la dernière migration** - Annuler la dernière migration créée
- **🗃️ EF - ⬆️ Mettre à jour la base de données** - Appliquer les migrations à la DB
- **🗃️ EF - ↩️ Annuler la dernière mise à jour** - Revenir à une migration précédente
- **🗃️ EF - 📑 Lister les migrations** - Afficher toutes les migrations disponibles

### Commandes en ligne

**Ajouter une migration :**
```bash
cd webapp
dotnet ef migrations add NomDeLaMigration
dotnet ef database update
```

**Build de production :**
```bash
dotnet publish -c Release
```

## Licence

Ce projet est distribué sous la licence MIT. Vous êtes libre de l'utiliser, le modifier et le distribuer, y compris pour des projets commerciaux, à condition de conserver la notice de copyright et la licence.

Voir le fichier [LICENSE](LICENSE) pour plus de détails.
