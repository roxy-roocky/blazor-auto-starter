using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace localization.Services;

/// <summary>
/// A StringLocalizerFactory that loads resources from the localization assembly.
/// This allows sharing resources between webapp and webapp.Client.
/// </summary>
public class SharedStringLocalizerFactory : IStringLocalizerFactory
{
    private static readonly Assembly ResourceAssembly = typeof(SharedStringLocalizerFactory).Assembly;

    public IStringLocalizer Create(Type resourceSource)
    {
        return new SharedStringLocalizer(resourceSource.FullName!);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new SharedStringLocalizer(baseName);
    }
}

public class SharedStringLocalizer : IStringLocalizer
{
    public static string Format(string template, params object[] args)
    {
        // Remplace {name} par {0}, {count} par {1}, etc.
        var index = 0;
        var pattern = @"\{([a-zA-Z_][a-zA-Z0-9_]*)\}";
        
        var formatted = Regex.Replace(template, pattern, match => 
        {
            return "{" + (index++) + "}";
        });
        
        return string.Format(formatted, args);
    }

    private readonly ResourceManager _resourceManager;

    public SharedStringLocalizer(string resourceName)
    {
        var resourceBaseName = $"localization.Resources.{resourceName}";
        _resourceManager = new ResourceManager(resourceBaseName, typeof(SharedStringLocalizerFactory).Assembly);
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = GetString(name);
            var formatted = value != null ? Format(value, arguments) : name;
            return new LocalizedString(name, formatted, value == null);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var resourceSet = _resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, includeParentCultures);
        if (resourceSet == null) yield break;

        foreach (System.Collections.DictionaryEntry entry in resourceSet)
        {
            var key = entry.Key.ToString()!;
            var value = entry.Value?.ToString() ?? key;
            yield return new LocalizedString(key, value);
        }
    }

    private string? GetString(string name)
    {
        try
        {
            return _resourceManager.GetString(name, CultureInfo.CurrentUICulture);
        }
        catch (MissingManifestResourceException)
        {
            return null;
        }
    }
}

public class SharedStringLocalizer<T> : SharedStringLocalizer, IStringLocalizer<T>
{
    public SharedStringLocalizer() : base(typeof(T).FullName!)
    {
    }
}
