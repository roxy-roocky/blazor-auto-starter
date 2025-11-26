using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

// Enregistre l'instance MSBuild pour pouvoir analyser les projets .NET
MSBuildLocator.RegisterDefaults();

return await RunAsync(args);

/// <summary>
/// Fonction principale du générateur de localisation
/// Analyse les projets pour extraire les clés de traduction et génère les fichiers .resx
/// </summary>
static async Task<int> RunAsync(string[] args)
{
    // Affiche l'aide si aucun argument ou si --help est demandé
    if (args.Length < 1 || args[0] == "-h" || args[0] == "--help")
    {
        Console.WriteLine("Usage: localization-generator [languages] <projects...>");
        Console.WriteLine("Example: localization-generator ../webapp.sln");
        Console.WriteLine("Example: localization-generator fr,en,es ../webapp.sln");
        Console.WriteLine();
        Console.WriteLine("If languages are not specified, uses configured cultures from LocalizationConfig");
        Console.WriteLine("Resources will be generated in localization/Resources (required by SharedStringLocalizer)");
        return 1;
    }

    // Détermine le répertoire de sortie - doit être localization/Resources pour que SharedStringLocalizer fonctionne
    var currentDirectory = Directory.GetCurrentDirectory();
    var outputDirectory = Path.GetFullPath(Path.Combine(currentDirectory, "localization", "Resources"));

    // Parse les arguments - les langues sont optionnelles
    string[] languages;
    string[] projectPaths;

    // Vérifie si le premier argument ressemble à un chemin de projet/solution
    var firstArg = args[0];
    if (firstArg.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) ||
        firstArg.EndsWith(".sln", StringComparison.OrdinalIgnoreCase) ||
        File.Exists(Path.GetFullPath(firstArg)))
    {
        // Aucune langue spécifiée, utilise la configuration
        languages = localization.Configuration.LocalizationConfig.GetCultureCodes();
        projectPaths = args.Select(Path.GetFullPath).ToArray();
        Console.WriteLine($"Using configured languages: {string.Join(", ", languages)}");
    }
    else
    {
        // Langues spécifiées en premier argument (ex: fr,en,es)
        languages = firstArg.Split(',').Select(l => l.Trim()).ToArray();
        projectPaths = args.Skip(1).Select(Path.GetFullPath).ToArray();
    }

    // Vérifie que tous les projets/solutions existent
    foreach (var projectPath in projectPaths)
    {
        if (!File.Exists(projectPath))
        {
            Console.WriteLine($"Error: Project/Solution '{projectPath}' does not exist");
            return 1;
        }
    }

    Console.WriteLine($"Output: {outputDirectory}");
    Console.WriteLine($"Languages: {string.Join(", ", languages)}");
    Console.WriteLine($"Projects: {string.Join(", ", projectPaths.Select(Path.GetFileName))}");
    Console.WriteLine();

    // Crée l'extracteur et le dictionnaire de résultats
    var extractor = new LocalizationExtractor();
    var results = new Dictionary<string, HashSet<string>>();

    // Analyse chaque projet/solution pour extraire les clés de localisation
    foreach (var projectPath in projectPaths)
    {
        Console.WriteLine($"Loading: {projectPath}");
        var projectResults = await extractor.ExtractFromProjectAsync(projectPath);

        // Fusionne les résultats de tous les projets
        foreach (var (key, value) in projectResults)
        {
            if (!results.ContainsKey(key))
                results[key] = new HashSet<string>();
            foreach (var v in value)
                results[key].Add(v);
        }
    }

    if (results.Count == 0)
    {
        Console.WriteLine("No localization keys found.");
        return 0;
    }

    Directory.CreateDirectory(outputDirectory);

    // Trouve et déplace les fichiers RESX obsolètes
    MoveObsoleteResxFiles(outputDirectory, results, languages);

    // Pour chaque type de ressource (ex: webapp.Components.Pages.LocalizationExample)
    foreach (var (resourceType, keys) in results)
    {
        Console.WriteLine($"Found {keys.Count} keys for {resourceType}");

        // Génère un fichier .resx pour chaque langue
        foreach (var language in languages)
        {
            // Utilise une structure plate avec le nom complet du type pour la convention .NET
            // ex: webapp.Components.Pages.LocalizationExample.fr.resx
            Directory.CreateDirectory(outputDirectory);
            var fileName = $"{resourceType}.{language}.resx";
            var filePath = Path.Combine(outputDirectory, fileName);

            // Charge les traductions existantes si le fichier existe déjà
            var existingTranslations = new Dictionary<string, string>();
            if (File.Exists(filePath))
            {
                var existingDoc = XDocument.Load(filePath);
                foreach (var data in existingDoc.Descendants("data"))
                {
                    var name = data.Attribute("name")?.Value;
                    var value = data.Element("value")?.Value;
                    if (name != null && value != null)
                    {
                        existingTranslations[name] = value;
                    }
                }
            }

            // Construit le dictionnaire final de traductions - garde uniquement les clés qui existent dans l'assemblage
            var finalTranslations = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                if (existingTranslations.ContainsKey(key))
                {
                    // Garde la traduction existante
                    finalTranslations[key] = existingTranslations[key];
                }
                else
                {
                    // Nouvelle clé - utilise la clé comme valeur par défaut
                    finalTranslations[key] = key;
                }
            }

            // Rapporte les clés supprimées (obsolètes)
            var removedKeys = existingTranslations.Keys.Except(keys).ToList();
            if (removedKeys.Count > 0)
            {
                Console.WriteLine($"  Removed {removedKeys.Count} obsolete key(s) from {fileName}:");
                foreach (var removedKey in removedKeys.OrderBy(k => k))
                {
                    Console.WriteLine($"    - {removedKey}");
                }
            }

            // Crée le fichier .resx avec l'en-tête standard XML
            var resx = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("root",
                    new XElement("resheader",
                        new XAttribute("name", "resmimetype"),
                        new XElement("value", "text/microsoft-resx")),
                    new XElement("resheader",
                        new XAttribute("name", "version"),
                        new XElement("value", "2.0")),
                    new XElement("resheader",
                        new XAttribute("name", "reader"),
                        new XElement("value", "System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")),
                    new XElement("resheader",
                        new XAttribute("name", "writer"),
                        new XElement("value", "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"))
                )
            );

            // Ajoute chaque traduction au fichier .resx (triées par ordre alphabétique)
            foreach (var (key, value) in finalTranslations.OrderBy(kv => kv.Key))
            {
                resx.Root!.Add(
                    new XElement("data",
                        new XAttribute("name", key),
                        new XAttribute(XNamespace.Xml + "space", "preserve"),
                        new XElement("value", value)
                    )
                );
            }

            // Sauvegarde le fichier .resx
            resx.Save(filePath);

            Console.WriteLine($"  Generated: {fileName}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("Done!");
    return 0;
}

/// <summary>
/// Déplace les fichiers .resx obsolètes dans un dossier "Obsolete"
/// Un fichier est considéré obsolète s'il ne correspond plus à aucun type de ressource actuel
/// </summary>
static void MoveObsoleteResxFiles(string outputDirectory, Dictionary<string, HashSet<string>> results, string[] languages)
{
    if (!Directory.Exists(outputDirectory))
        return;

    // Construit un ensemble des noms de fichiers .resx attendus basés sur les types de ressources actuels
    var expectedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var resourceType in results.Keys)
    {
        foreach (var language in languages)
        {
            expectedFiles.Add($"{resourceType}.{language}.resx");
        }
    }

    // Trouve tous les fichiers .resx existants
    var existingFiles = Directory.GetFiles(outputDirectory, "*.resx", SearchOption.TopDirectoryOnly);

    // Identifie les fichiers obsolètes (non attendus)
    var obsoleteFiles = existingFiles
        .Select(Path.GetFileName)
        .Where(fileName => fileName != null && !expectedFiles.Contains(fileName!))
        .ToList();

    if (obsoleteFiles.Count == 0)
        return;

    // Crée le répertoire "Obsolete"
    var obsoleteDir = Path.Combine(outputDirectory, "Obsolete");
    Directory.CreateDirectory(obsoleteDir);

    Console.WriteLine($"Moving {obsoleteFiles.Count} obsolete RESX file(s) to {Path.GetFileName(obsoleteDir)}/:");
    foreach (var fileName in obsoleteFiles.OrderBy(f => f))
    {
        var sourcePath = Path.Combine(outputDirectory, fileName!);
        var destPath = Path.Combine(obsoleteDir, fileName!);

        // Si le fichier existe déjà dans le dossier obsolete, ajoute un timestamp
        if (File.Exists(destPath))
        {
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            destPath = Path.Combine(obsoleteDir, $"{nameWithoutExt}.{timestamp}{ext}");
        }

        File.Move(sourcePath, destPath);
        Console.WriteLine($"  Moved: {fileName}");
    }
    Console.WriteLine();
}

/// <summary>
/// Classe responsable de l'extraction des clés de localisation depuis le code source
/// Utilise Roslyn pour analyser les fichiers C# et des regex pour les fichiers Razor
/// </summary>
public class LocalizationExtractor
{
    // Type de base pour les attributs de validation
    private const string ValidationAttributeBase = "System.ComponentModel.DataAnnotations.ValidationAttribute";

    /// <summary>
    /// Extrait les clés de localisation depuis un projet ou une solution
    /// </summary>
    /// <returns>Dictionnaire: Type de ressource → Ensemble de clés de localisation</returns>
    public async Task<Dictionary<string, HashSet<string>>> ExtractFromProjectAsync(string projectPath)
    {
        var results = new Dictionary<string, HashSet<string>>();

        // Crée un workspace MSBuild pour analyser les projets
        using var workspace = MSBuildWorkspace.Create();
        workspace.RegisterWorkspaceFailedHandler((e) =>
        {
            if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                Console.WriteLine($"Warning: {e.Diagnostic.Message}");
        });

        List<Project> projects;

        // Charge soit une solution complète, soit un projet unique
        if (projectPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
        {
            var solution = await workspace.OpenSolutionAsync(projectPath);
            projects = solution.Projects.ToList();
        }
        else
        {
            var project = await workspace.OpenProjectAsync(projectPath);
            projects = new List<Project> { project };
        }

        // Analyse chaque projet
        foreach (var project in projects)
        {
            Console.WriteLine($"Analyzing project: {project.Name}");

            var compilation = await project.GetCompilationAsync();
            if (compilation == null) continue;

            // Analyse chaque fichier source dans le projet
            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                var root = await tree.GetRootAsync();

                // Extrait les utilisations de IStringLocalizer
                ExtractLocalizerUsages(root, semanticModel, results);

                // Extrait depuis les Data Annotations avec analyse sémantique
                ExtractFromDataAnnotations(root, semanticModel, tree.FilePath, results);
            }

            // Scanne aussi les fichiers .razor (non inclus dans la compilation Roslyn)
            var projectDir = Path.GetDirectoryName(project.FilePath)!;
            ExtractFromRazorFiles(projectDir, project.Name, results);
        }

        return results;
    }

    /// <summary>
    /// Parcourt récursivement tous les fichiers .razor dans un répertoire
    /// </summary>
    public void ExtractFromRazorFiles(string directory, string projectName, Dictionary<string, HashSet<string>> results)
    {
        foreach (var file in Directory.GetFiles(directory, "*.razor", SearchOption.AllDirectories))
        {
            ExtractFromRazorFile(file, directory, projectName, results);
        }
    }

    /// <summary>
    /// Extrait les utilisations de IStringLocalizer dans les fichiers C#
    /// Cherche les patterns comme: localizer["Key"] ou _localizer["Key", args]
    /// </summary>
    private void ExtractLocalizerUsages(SyntaxNode root, SemanticModel semanticModel, Dictionary<string, HashSet<string>> results)
    {
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var classDecl in classDeclarations)
        {
            // Trouve tous les champs/propriétés de type IStringLocalizer<T>
            var localizerTypes = FindLocalizerTypes(classDecl, semanticModel);

            foreach (var (fieldName, resourceType) in localizerTypes)
            {
                // Extrait toutes les clés utilisées avec ce localizer
                var keys = ExtractLocalizerKeys(classDecl, fieldName);

                if (keys.Count > 0)
                {
                    if (!results.ContainsKey(resourceType))
                        results[resourceType] = new HashSet<string>();

                    foreach (var key in keys)
                        results[resourceType].Add(key);
                }
            }
        }
    }

    /// <summary>
    /// Extrait les clés de localisation depuis un fichier .razor
    /// Utilise des expressions régulières car Roslyn ne compile pas les fichiers Razor
    /// </summary>
    private void ExtractFromRazorFile(string filePath, string projectDirectory, string projectName, Dictionary<string, HashSet<string>> results)
    {
        var content = File.ReadAllText(filePath);

        // Trouve les patterns @inject IStringLocalizer<T> variableName
        var injectPattern = new Regex(@"@inject\s+IStringLocalizer<(\w+)>\s+(\w+)");
        var injectMatches = injectPattern.Matches(content);

        foreach (Match match in injectMatches)
        {
            var typeName = match.Groups[1].Value;
            var variableName = match.Groups[2].Value;

            // Construit le namespace complet depuis le chemin du fichier
            // ex: webapp/Components/Pages/LocalizationExample → webapp.Components.Pages.LocalizationExample
            var relativePath = Path.GetRelativePath(projectDirectory, filePath);
            var relativeDir = Path.GetDirectoryName(relativePath) ?? "";
            var namespaceParts = relativeDir.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Where(p => !string.IsNullOrEmpty(p));
            var fullNamespace = string.Join(".", new[] { projectName }.Concat(namespaceParts).Append(typeName));

            // Trouve toutes les utilisations: variableName["Key"] ou variableName["Key", args]
            var usagePattern = new Regex($@"{variableName}\[""([^""]+)""[,\]]");
            var usageMatches = usagePattern.Matches(content);

            if (usageMatches.Count > 0)
            {
                if (!results.ContainsKey(fullNamespace))
                    results[fullNamespace] = new HashSet<string>();

                foreach (Match usageMatch in usageMatches)
                {
                    results[fullNamespace].Add(usageMatch.Groups[1].Value);
                }
            }
        }
    }

    /// <summary>
    /// Trouve tous les IStringLocalizer<T> dans une classe
    /// Cherche dans les champs, propriétés et paramètres de constructeur (injection de dépendances)
    /// </summary>
    /// <returns>Liste de tuples (nom de la variable, type de ressource complet)</returns>
    private List<(string FieldName, string ResourceType)> FindLocalizerTypes(ClassDeclarationSyntax classDecl, SemanticModel semanticModel)
    {
        var result = new List<(string, string)>();

        // Vérifie les champs de classe
        foreach (var field in classDecl.DescendantNodes().OfType<FieldDeclarationSyntax>())
        {
            var typeInfo = semanticModel.GetTypeInfo(field.Declaration.Type);
            var typeName = typeInfo.Type?.ToDisplayString() ?? "";

            if (typeName.StartsWith("Microsoft.Extensions.Localization.IStringLocalizer<"))
            {
                var resourceType = ExtractGenericArgument(typeName);
                foreach (var variable in field.Declaration.Variables)
                {
                    result.Add((variable.Identifier.Text, resourceType));
                }
            }
        }

        // Vérifie les propriétés
        foreach (var prop in classDecl.DescendantNodes().OfType<PropertyDeclarationSyntax>())
        {
            var typeInfo = semanticModel.GetTypeInfo(prop.Type);
            var typeName = typeInfo.Type?.ToDisplayString() ?? "";

            if (typeName.StartsWith("Microsoft.Extensions.Localization.IStringLocalizer<"))
            {
                var resourceType = ExtractGenericArgument(typeName);
                result.Add((prop.Identifier.Text, resourceType));
            }
        }

        // Vérifie les paramètres de constructeur (pour l'injection de dépendances)
        foreach (var ctor in classDecl.DescendantNodes().OfType<ConstructorDeclarationSyntax>())
        {
            foreach (var param in ctor.ParameterList.Parameters)
            {
                if (param.Type == null) continue;

                var typeInfo = semanticModel.GetTypeInfo(param.Type);
                var typeName = typeInfo.Type?.ToDisplayString() ?? "";

                if (typeName.StartsWith("Microsoft.Extensions.Localization.IStringLocalizer<"))
                {
                    var resourceType = ExtractGenericArgument(typeName);
                    result.Add((param.Identifier.Text, resourceType));
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Extrait l'argument générique de IStringLocalizer<T>
    /// Ex: "Microsoft.Extensions.Localization.IStringLocalizer<webapp.Pages.Home>" → "webapp.Pages.Home"
    /// </summary>
    private string ExtractGenericArgument(string fullTypeName)
    {
        // Extrait depuis "Microsoft.Extensions.Localization.IStringLocalizer<Namespace.Type>"
        var start = fullTypeName.IndexOf('<') + 1;
        var end = fullTypeName.LastIndexOf('>');
        if (start > 0 && end > start)
        {
            // Retourne le nom complet namespace.type
            return fullTypeName.Substring(start, end - start);
        }
        return "Unknown";
    }

    /// <summary>
    /// Extrait toutes les clés de localisation utilisées avec un localizer donné
    /// Cherche les patterns: localizer["Key"] ou _localizer["Key", args]
    /// </summary>
    private HashSet<string> ExtractLocalizerKeys(ClassDeclarationSyntax classDecl, string fieldName)
    {
        var keys = new HashSet<string>();

        // Trouve les patterns comme: fieldName["Key"] ou fieldName["Key", args]
        var elementAccesses = classDecl.DescendantNodes().OfType<ElementAccessExpressionSyntax>();

        foreach (var access in elementAccesses)
        {
            var expression = access.Expression.ToString();

            // Match le nom du champ (peut être _localizer, localizer, Localizer, etc.)
            if (expression == fieldName || expression == $"_{fieldName}" || expression == $"this.{fieldName}")
            {
                if (access.ArgumentList.Arguments.Count > 0)
                {
                    var firstArg = access.ArgumentList.Arguments[0].Expression;
                    if (firstArg is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
                    {
                        keys.Add(literal.Token.ValueText);
                    }
                }
            }
        }

        return keys;
    }

    /// <summary>
    /// Extrait les clés de localisation depuis les attributs de validation (Data Annotations)
    /// Ex: [Required(ErrorMessage = "Email is required")] → "Email is required"
    /// </summary>
    private void ExtractFromDataAnnotations(SyntaxNode root, SemanticModel semanticModel, string filePath, Dictionary<string, HashSet<string>> results)
    {
        var attributes = root.DescendantNodes().OfType<AttributeSyntax>();

        // Détermine le type de ressource depuis le nom complet de la classe
        var classDecl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDecl == null) return;

        var classSymbol = semanticModel.GetDeclaredSymbol(classDecl);
        var className = classSymbol?.ToDisplayString() ?? classDecl.Identifier.Text;

        var annotationKeys = new HashSet<string>();

        foreach (var attr in attributes)
        {
            // Obtient le type d'attribut via l'analyse sémantique
            var symbolInfo = semanticModel.GetSymbolInfo(attr);
            var attributeSymbol = symbolInfo.Symbol?.ContainingType;

            if (attributeSymbol == null) continue;

            // Vérifie si cet attribut dérive de ValidationAttribute
            if (DerivesFrom(attributeSymbol, ValidationAttributeBase))
            {
                // Extrait ErrorMessage de tout dérivé de ValidationAttribute
                var errorMessage = GetAttributeArgument(attr, "ErrorMessage");
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    annotationKeys.Add(errorMessage);
                }
            }

            // Note: Les attributs Display (Name, Description, etc.) ne sont pas extraits
            // car ils ne peuvent pas être localisés dans Blazor sans IStringLocalizer
        }

        if (annotationKeys.Count > 0)
        {
            var resourceType = $"DataAnnotations.{className}";
            if (!results.ContainsKey(resourceType))
                results[resourceType] = new HashSet<string>();

            foreach (var key in annotationKeys)
                results[resourceType].Add(key);
        }
    }

    /// <summary>
    /// Vérifie si un type dérive d'un type de base donné
    /// Remonte la chaîne d'héritage pour trouver une correspondance
    /// </summary>
    private bool DerivesFrom(INamedTypeSymbol type, string baseTypeName)
    {
        var current = type;
        while (current != null)
        {
            if (current.ToDisplayString() == baseTypeName)
                return true;
            current = current.BaseType;
        }
        return false;
    }

    /// <summary>
    /// Récupère la valeur d'un argument nommé d'un attribut
    /// Ex: [Required(ErrorMessage = "Error")] → GetAttributeArgument(..., "ErrorMessage") retourne "Error"
    /// </summary>
    private string? GetAttributeArgument(AttributeSyntax attr, string argumentName)
    {
        if (attr.ArgumentList == null) return null;

        foreach (var arg in attr.ArgumentList.Arguments)
        {
            // Argument nommé
            if (arg.NameEquals?.Name.ToString() == argumentName)
            {
                if (arg.Expression is LiteralExpressionSyntax literal)
                {
                    return literal.Token.ValueText;
                }
            }
        }

        return null;
    }
}
