using Microsoft.Extensions.DependencyInjection;

namespace html_generator;

/// <summary>
/// Extension methods for configuring HTML rendering services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Blazor HTML rendering services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBlazorHtmlGenerator(this IServiceCollection services)
    {
        services.AddScoped<HtmlRendererService>();

        return services;
    }
}
