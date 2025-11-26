using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddSingleton<webapp.Client.Managers.ThemeManager>();
builder.Services.AddSingleton<webapp.Client.Managers.CultureManager>();

// Localization
builder.Services.AddSingleton<Microsoft.Extensions.Localization.IStringLocalizerFactory, localization.Services.SharedStringLocalizerFactory>();
builder.Services.AddSingleton(typeof(Microsoft.Extensions.Localization.IStringLocalizer<>), typeof(localization.Services.SharedStringLocalizer<>));

var host = builder.Build();

// Apply culture from cookie at startup
var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
var cultureCookie = await jsRuntime.InvokeAsync<string?>("CultureSwitcher.getCurrentCultureFromCookie");

if (!string.IsNullOrEmpty(cultureCookie) && localization.Configuration.LocalizationConfig.IsSupported(cultureCookie))
{
    var cultureInfo = new CultureInfo(cultureCookie);
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
}

await host.RunAsync();
