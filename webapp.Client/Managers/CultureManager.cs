using System.Globalization;
using Microsoft.JSInterop;

namespace webapp.Client.Managers;

public class CultureManager
{
    protected readonly IJSRuntime _jsRuntime;
    protected readonly ILogger<CultureManager> _logger;
    private string? _currentCulture;
    private bool _initialized = false;

    public static string[] SupportedCultures => localization.Configuration.LocalizationConfig.GetCultureCodes();
    public static string DefaultCulture => localization.Configuration.LocalizationConfig.DefaultCulture;

    public CultureManager(IJSRuntime jsRuntime, ILogger<CultureManager> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public event EventHandler<string>? CultureChanged;

    public async Task<string> GetCurrentCultureAsync()
    {
        if (!_initialized)
        {
            await InitializeCultureAsync();
        }

        return _currentCulture ?? DefaultCulture;
    }

    private async Task InitializeCultureAsync()
    {
        try
        {
            var cultureValue = await _jsRuntime.InvokeAsync<string?>("CultureSwitcher.getCurrentCultureFromCookie");
            if (!string.IsNullOrEmpty(cultureValue) && SupportedCultures.Contains(cultureValue))
            {
                _currentCulture = cultureValue;
            }
            else
            {
                // Try to get browser language
                var browserLang = await _jsRuntime.InvokeAsync<string?>("CultureSwitcher.getBrowserLanguage");
                if (!string.IsNullOrEmpty(browserLang))
                {
                    var lang = browserLang.Split('-')[0].ToLowerInvariant();
                    _currentCulture = SupportedCultures.Contains(lang) ? lang : DefaultCulture;
                }
                else
                {
                    _currentCulture = DefaultCulture;
                }
            }

            ApplyCulture(_currentCulture);
            _initialized = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch culture from cookie.");
            _currentCulture = DefaultCulture;
            _initialized = true;
        }
    }

    public virtual async Task SetCultureAsync(string culture)
    {
        if (!SupportedCultures.Contains(culture))
        {
            _logger.LogWarning("Culture {Culture} is not supported.", culture);
            return;
        }

        _currentCulture = culture;
        ApplyCulture(culture);
        await _jsRuntime.InvokeVoidAsync("CultureSwitcher.setCulture", culture);
        CultureChanged?.Invoke(this, culture);
    }

    private void ApplyCulture(string culture)
    {
        var cultureInfo = new CultureInfo(culture);
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }

    public static string GetDisplayName(string culture) =>
        localization.Configuration.LocalizationConfig.GetDisplayName(culture);

    public static string GetFlag(string culture) =>
        localization.Configuration.LocalizationConfig.GetFlag(culture);
}
