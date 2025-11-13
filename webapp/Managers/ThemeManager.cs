using System;
using Microsoft.JSInterop;
using Theme = webapp.Client.Managers.ThemeManager.Theme;

namespace webapp.Managers;

public class ThemeManager
{
    protected readonly IHttpContextAccessor _httpContextAccessor;

    public ThemeManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual Theme GetCurrentTheme()
    {
        if (_currentTheme is null && _httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue("theme", out string? theme) && theme is not null)
        {
            _currentTheme = Enum.Parse<Theme>(theme);
        }
        
        return _currentTheme ?? Theme.Light;
    }
    private Theme? _currentTheme;
}
