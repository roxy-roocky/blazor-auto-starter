namespace html_generator.Emails;

/// <summary>
/// Configuration centralisée pour les styles des templates d'email
/// </summary>
public static class EmailStyleConfig
{
    // Layout colors
    public static string LayoutBackgroundColor { get; set; } = "#e6cfbc";
    public static string ContentBackgroundColor { get; set; } = "#ffffff";
    public static string HeaderBackgroundColor { get; set; } = "#5f2f21";
    public static string FooterBackgroundColor { get; set; } = "#333333";
    public static string FooterTextColor { get; set; } = "#ffffff";

    // Button colors
    public static string ButtonBackgroundColor { get; set; } = "#975b45";
    public static string ButtonTextColor { get; set; } = "#ffffff";
    public static string ButtonSubTextColor { get; set; } = "#999999";

    // Text colors
    public static string HeadingColor { get; set; } = "#333333";
    public static string TextColor { get; set; } = "#666666";

    // Fonts
    public static string FontFamily { get; set; } = "Arial, sans-serif";
    public static string FontSize { get; set; } = "16px";
    public static string HeadingSize { get; set; } = "22px";
    public static string ButtonFontWeight { get; set; } = "bold";
}
