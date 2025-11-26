namespace localization.Configuration;

public record CultureInfo(
    string Code,
    string DisplayName,
    string Flag
);

public static class LocalizationConfig
{
    public static readonly CultureInfo[] SupportedCultures =
    [
        new("en", "English", "🇬🇧"),
        new("fr", "Français", "🇫🇷")
    ];

    public static readonly string DefaultCulture = "en";

    // Méthodes utilitaires
    public static string[] GetCultureCodes() => SupportedCultures.Select(c => c.Code).ToArray();

    public static string GetDisplayName(string code) =>
        SupportedCultures.FirstOrDefault(c => c.Code == code)?.DisplayName ?? code;

    public static string GetFlag(string code) =>
        SupportedCultures.FirstOrDefault(c => c.Code == code)?.Flag ?? "🌐";

    public static bool IsSupported(string code) =>
        SupportedCultures.Any(c => c.Code == code);
}