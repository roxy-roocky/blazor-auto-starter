using System;
using Microsoft.JSInterop;

namespace webapp.Client.Managers;

public class ThemeManager
{
    public enum Theme { Light = 0, Dark = 1 };
    protected readonly IJSRuntime _jsRuntime;
    protected ILogger<ThemeManager> _logger;
    private Theme? _currentTheme;
    private bool _initialized = false;

    public ThemeManager(IJSRuntime jsRuntime, ILogger<ThemeManager> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public event EventHandler<Theme>? ThemeChanged;

    public async Task<Theme> GetCurrentThemeAsync()
    {
        if (!_initialized)
        {
            await InitializeThemeAsync();
        }

        return _currentTheme ?? Theme.Light;
    }

    private async Task InitializeThemeAsync()
    {
        try
        {
            var themeValue = await _jsRuntime.InvokeAsync<int?>("ThemeSwitcher.getCurrentThemeFromCookie");
            if (themeValue.HasValue)
            {
                _currentTheme = (Theme)themeValue.Value;
            }
            _initialized = true;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch theme from cookie.");
            _currentTheme = Theme.Light;
            _initialized = true;
        }
    }

    public virtual async Task SwitchTheme(Theme newTheme)
    {
        _currentTheme = newTheme;
        await _jsRuntime.InvokeVoidAsync("ThemeSwitcher.setTheme", newTheme);
        ThemeChanged?.Invoke(this, newTheme);
    }
}
