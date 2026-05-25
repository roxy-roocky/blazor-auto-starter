# Système de Localisation - Documentation

## Vue d'ensemble

Ce document décrit le système de localisation mis en place pour le projet Blazor Auto Starter. Le système utilise les fichiers .resx natifs de .NET avec des satellite assemblies pour un chargement optimisé, notamment en WebAssembly.

---

## Architecture

```
blazor-auto-starter/
├── localization/                    # Projet contenant les ressources
│   ├── Resources/                   # Fichiers .resx générés
│   │   ├── webapp.Components.Pages.LocalizationExample.fr.resx
│   │   ├── webapp.Components.Pages.LocalizationExample.en.resx
│   │   └── ...
│   └── Services/
│       └── SharedStringLocalizerFactory.cs  # Factory custom
├── localization-generator/          # Outil de génération des .resx
│   └── Program.cs
├── webapp/                          # Projet serveur
└── webapp.Client/                   # Projet WASM
```

---

## Composants du système

### 1. Générateur de ressources (`localization-generator`)

**Rôle :** Scanner le code source pour extraire automatiquement les clés de localisation et générer les fichiers .resx.

**Fonctionnalités :**
- Analyse sémantique avec Roslyn (MSBuildWorkspace)
- Détection des `IStringLocalizer<T>` dans les fichiers .cs et .razor
- Extraction des clés depuis les appels `localizer["Key"]`
- Détection des Data Annotations (tous les dérivés de `ValidationAttribute`)
- Préservation des traductions existantes lors de la regénération

**Usage :**
```bash
cd localization-generator
dotnet run <output-directory> <languages> <projects...>

# Exemples :
dotnet run ../localization/Resources fr,en ../webapp/webapp.csproj
dotnet run ../localization/Resources fr,en,es ../webapp/webapp.csproj ../webapp.Client/webapp.Client.csproj
```

**Ce que l'outil détecte :**

1. **IStringLocalizer dans les fichiers .razor :**
```razor
@inject IStringLocalizer<LocalizationExample> L

<h1>@L["PageTitle"]</h1>
<p>@L["WelcomeMessage"]</p>
```

2. **IStringLocalizer dans les fichiers .cs :**
```csharp
public class MyService
{
    private readonly IStringLocalizer<MyService> _localizer;

    public string GetMessage() => _localizer["HelloWorld"];
}
```

3. **Data Annotations :**
```csharp
public class LoginModel
{
    [Required(ErrorMessage = "EmailRequired")]
    [Display(Name = "EmailLabel")]
    public string Email { get; set; }
}
```

---

### 2. SharedStringLocalizerFactory

**Fichier :** `localization/Services/SharedStringLocalizerFactory.cs`

**Rôle :** Charger les ressources depuis l'assembly `localization` plutôt que depuis l'assembly où le type T est défini.

**Pourquoi c'est nécessaire :**
- Par défaut, `IStringLocalizer<T>` cherche les ressources dans l'assembly de T
- Nos ressources sont centralisées dans le projet `localization`
- Cette factory redirige la recherche vers le bon assembly

**Fonctionnement :**
```csharp
public class SharedStringLocalizerFactory : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource)
    {
        // Charge depuis localization.Resources.{FullTypeName}
        return new SharedStringLocalizer(resourceSource.FullName!);
    }
}
```

---

### 3. Configuration des projets

#### webapp/Program.cs
```csharp
// Enregistrement du factory custom
builder.Services.AddSingleton<IStringLocalizerFactory, SharedStringLocalizerFactory>();
```

#### webapp.Client/Program.cs
```csharp
// Même configuration côté WASM
builder.Services.AddSingleton<IStringLocalizerFactory, SharedStringLocalizerFactory>();
```

#### webapp.Client.csproj
```xml
<PropertyGroup>
  <!-- Active le support complet de la globalisation en WASM -->
  <BlazorWebAssemblyLoadAllGlobalizationData>true</BlazorWebAssemblyLoadAllGlobalizationData>
</PropertyGroup>
```

---

## Format des fichiers .resx

Les fichiers suivent la convention de nommage .NET :
- `{FullTypeName}.{culture}.resx`

**Exemple :** `webapp.Components.Pages.LocalizationExample.fr.resx`

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <data name="PageTitle" xml:space="preserve">
    <value>Titre de la page</value>
  </data>
  <data name="WelcomeMessage" xml:space="preserve">
    <value>Bienvenue sur notre site</value>
  </data>
</root>
```

---

## Satellite Assemblies

### Comment ça marche

Lors de la compilation, .NET génère des **satellite assemblies** pour chaque culture :

```
localization/bin/Debug/net9.0/
├── localization.dll              # Assembly principal (culture neutre)
├── fr/
│   └── localization.resources.dll  # Satellite français
└── en/
    └── localization.resources.dll  # Satellite anglais
```

### Avantages pour WebAssembly

- **Chargement à la demande :** Seul le satellite de la culture active est téléchargé
- **Réduction de la taille initiale :** L'utilisateur ne télécharge pas toutes les langues
- **Mise en cache :** Les satellites peuvent être mis en cache par le navigateur

---

## Utilisation dans les composants

### Composant Razor (.razor)

```razor
@page "/example"
@using Microsoft.Extensions.Localization
@inject IStringLocalizer<LocalizationExample> L

<h1>@L["PageTitle"]</h1>

<p>@L["WelcomeMessage"]</p>

<!-- Avec paramètres -->
<p>@L["GreetingWithName", userName]</p>

<!-- Avec vérification -->
@if (!L["OptionalMessage"].ResourceNotFound)
{
    <p>@L["OptionalMessage"]</p>
}
```

### Service C# (.cs)

```csharp
public class NotificationService
{
    private readonly IStringLocalizer<NotificationService> _localizer;

    public NotificationService(IStringLocalizer<NotificationService> localizer)
    {
        _localizer = localizer;
    }

    public string GetSuccessMessage()
    {
        return _localizer["OperationSuccessful"];
    }
}
```

---

## Workflow de développement

### 1. Ajouter des textes localisables

Dans votre composant :
```razor
@inject IStringLocalizer<MyComponent> L

<h1>@L["NewFeatureTitle"]</h1>
```

### 2. Générer les fichiers .resx

```bash
cd localization-generator
dotnet run ../localization/Resources fr,en ../webapp/webapp.csproj
```

**Output :**
```
Found 3 keys for webapp.Components.Pages.MyComponent
  Generated: webapp.Components.Pages.MyComponent.fr.resx
  Generated: webapp.Components.Pages.MyComponent.en.resx
```

### 3. Traduire les fichiers

Éditer les fichiers .resx générés et remplacer les valeurs par les traductions :

**Avant :**
```xml
<data name="NewFeatureTitle" xml:space="preserve">
  <value>NewFeatureTitle</value>
</data>
```

**Après (fr) :**
```xml
<data name="NewFeatureTitle" xml:space="preserve">
  <value>Nouvelle fonctionnalité</value>
</data>
```

### 4. Recompiler

```bash
dotnet build
```

Les traductions seront automatiquement compilées en satellite assemblies.

---

## Changer la culture à l'exécution

### Côté serveur

```csharp
// Dans un middleware ou contrôleur
CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
```

### Côté WASM

```csharp
// Dans Program.cs ou un service
var culture = new CultureInfo("fr-FR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
```

### Via les préférences du navigateur

Le runtime WASM utilise par défaut la langue du navigateur.

---

## Bonnes pratiques

### Nommage des clés

- Utiliser des noms descriptifs : `LoginButton`, `EmailRequired`
- Éviter les phrases complètes comme clés : ❌ `"Please enter your email"`
- Grouper par fonctionnalité : `Login_EmailLabel`, `Login_PasswordLabel`

### Organisation des ressources

- Une ressource par composant/service pour `IStringLocalizer<T>`
- Ressources partagées dans un type dédié : `IStringLocalizer<SharedResources>`

### Performance

- Les ressources sont mises en cache par le `ResourceManager`
- Éviter de créer des `IStringLocalizer` dans des boucles serrées

---

## Dépannage

### Les traductions ne s'affichent pas

1. Vérifier que le fichier .resx existe avec le bon nom
2. Vérifier que la culture est correctement définie
3. Vérifier les logs pour `MissingManifestResourceException`

### Le générateur ne trouve pas les clés

1. Vérifier que le projet compile sans erreurs
2. S'assurer que `IStringLocalizer<T>` est bien injecté
3. Utiliser des string literals (pas de variables) pour les clés

### Erreur de chargement en WASM

1. Vérifier que `BlazorWebAssemblyLoadAllGlobalizationData` est `true`
2. Vérifier que le projet `localization` est bien référencé
3. Vider le cache du navigateur

---

## Fichiers modifiés

| Fichier | Description |
|---------|-------------|
| `localization/localization.csproj` | Nettoyé, utilise uniquement Microsoft.Extensions.Localization |
| `localization/Services/SharedStringLocalizerFactory.cs` | Factory custom pour charger les ressources |
| `webapp/Program.cs` | Enregistrement du SharedStringLocalizerFactory |
| `webapp.Client/Program.cs` | Enregistrement du SharedStringLocalizerFactory |
| `webapp.Client/webapp.Client.csproj` | Activation de la globalisation WASM |
| `localization-generator/Program.cs` | Outil de génération des .resx |

---

## Commandes utiles

```bash
# Générer les ressources
cd localization-generator
dotnet run ../localization/Resources fr,en ../webapp/webapp.csproj

# Compiler le projet
dotnet build webapp/webapp.csproj

# Lister les fichiers générés
ls localization/Resources/

# Voir le contenu d'un fichier .resx
cat localization/Resources/webapp.Components.Pages.LocalizationExample.fr.resx
```
