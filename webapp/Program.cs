using html_generator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using webapp.Client.Pages;
using webapp.Components;
using webapp.Components.Account;
using webapp.Components.Pages;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<main_datacontext.MainDbContext>(options =>
    options.UseSqlite(connectionString, b => b.MigrationsAssembly("webapp")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<main_datacontext.Identity.User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<main_datacontext.MainDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IEmailSender<main_datacontext.Identity.User>, webapp.Managers.Account.EmailManager>();

builder.Services.AddScoped<webapp.Managers.ThemeManager>();
builder.Services.AddScoped<webapp.Client.Managers.CultureManager>();

// Localization
builder.Services.AddSingleton<Microsoft.Extensions.Localization.IStringLocalizerFactory, localization.Services.SharedStringLocalizerFactory>();
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = localization.Configuration.LocalizationConfig.GetCultureCodes();
    options.SetDefaultCulture(localization.Configuration.LocalizationConfig.DefaultCulture)
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);

    // Use cookie to determine culture
    options.RequestCultureProviders.Insert(0, new Microsoft.AspNetCore.Localization.CustomRequestCultureProvider(context =>
    {
        var cultureCookie = context.Request.Cookies["culture"];
        if (!string.IsNullOrEmpty(cultureCookie) && localization.Configuration.LocalizationConfig.IsSupported(cultureCookie))
        {
            return Task.FromResult<Microsoft.AspNetCore.Localization.ProviderCultureResult?>(
                new Microsoft.AspNetCore.Localization.ProviderCultureResult(cultureCookie));
        }
        return Task.FromResult<Microsoft.AspNetCore.Localization.ProviderCultureResult?>(null);
    }));
});

// Ajout des services

// Ajout des jobCode
builder.Services.AddBlazorHtmlGenerator();

if(builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<jobcode.Managers.Emails.IEmailSender, jobcode.Managers.Emails.DbEmailSender>();
}
else
{
    builder.Services.AddScoped<jobcode.Managers.Emails.IEmailSender, jobcode.Managers.Emails.BrevoEmailSender>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(webapp.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
