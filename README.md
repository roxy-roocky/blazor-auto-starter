# Blazor Auto Starter

Template de démarrage moderne pour application web Blazor .NET 9 avec mode de rendu automatique (Auto), combinant Server-Side Rendering (SSR) et WebAssembly pour des performances optimales.

## Fonctionnalités

- 🔐 **Authentification complète** - Système d'identité ASP.NET Core avec gestion des utilisateurs
- 🎨 **Thèmes personnalisables** - Basculement dynamique entre thème clair et sombre avec persistance par cookies
- ⚡ **Rendu hybride** - Mode Auto combinant rendu serveur (InteractiveServer) et WebAssembly (InteractiveWebAssembly)
- 💾 **Base de données SQLite** - Entity Framework Core avec migrations pour la gestion des données
- 🎭 **Bootstrap 5** - Interface utilisateur responsive avec intégration Bootstrap 5.3
- ✨ **Font Awesome** - Bibliothèque d'icônes complète (v7.0)
- 🔧 **SASS** - Compilation automatique des styles SASS
- 🏗️ **Architecture modulaire** - Séparation claire entre client, serveur et couche de données

## Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
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

Éditez ensuite ces fichiers selon vos besoins (chaînes de connexion, paramètres d'environnement, etc.).

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

Le projet est organisé en trois projets distincts :

#### **webapp** - Application serveur principale
- **Components/** - Composants Razor partagés
  - **Account/** - Composants d'authentification et gestion de compte
  - **Layout/** - Composants de mise en page (navigation, footer, etc.)
  - **Pages/** - Pages principales (Home, Weather, Error)
- **Managers/** - Services et gestionnaires (ex: ThemeManager)
- **wwwroot/** - Ressources statiques
  - **js/** - Scripts JavaScript (ThemeSwitcher.js)
  - **bootstrap/** - Fichiers Bootstrap compilés
  - **fontawesome/** - Styles et webfonts Font Awesome
- **Styles/** - Fichiers sources SASS
- **Program.cs** - Point d'entrée et configuration des services

#### **webapp.Client** - Application WebAssembly
- **Components/** - Composants côté client
- **Managers/** - Gestionnaires côté client
- **Pages/** - Pages rendues côté client
- **wwwroot/** - Ressources statiques spécifiques au client
- **Program.cs** - Configuration WebAssembly

#### **main-datacontext** - Couche de données
- Contexte Entity Framework Core
- Modèles de données
- Configuration Identity
- Gestion des migrations

### 🔄 Flux de compilation

1. **SASS** : Les fichiers `.scss` dans `Styles/` sont compilés vers `wwwroot/`
2. **Bootstrap** : Les fichiers JavaScript sont copiés depuis `node_modules/`
3. **Font Awesome** : SASS compilé et webfonts copiés automatiquement
4. **Blazor** : Compilation des composants Razor en C#

## Technologies utilisées

- **Framework** : .NET 9
- **UI** : Blazor (mode Auto - Server + WebAssembly)
- **Base de données** : SQLite avec Entity Framework Core 9
- **Authentification** : ASP.NET Core Identity
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
