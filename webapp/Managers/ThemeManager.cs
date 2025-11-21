using System;
using Microsoft.JSInterop;
using Theme = webapp.Client.Managers.ThemeManager.Theme;

namespace webapp.Managers;

public class ThemeManager
{
    protected readonly ILogger<ThemeManager> _logger;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    public ThemeManager(IHttpContextAccessor httpContextAccessor, ILogger<ThemeManager> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public virtual Theme GetCurrentTheme()
    {
        if (_currentTheme is null && _httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue("theme", out string? theme) && theme is not null)
        {
            _logger.LogInformation("Theme detected: {Theme}", theme);
            _currentTheme = Enum.Parse<Theme>(theme);
        }

        if(_currentTheme is null) 
        {
            _currentTheme = Theme.Light;
            _logger.LogInformation("Theme fallback: {Theme}", _currentTheme);
        }
        
        return _currentTheme.Value;
    }
    private Theme? _currentTheme;
}
