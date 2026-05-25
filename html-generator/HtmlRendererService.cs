using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace html_generator;

/// <summary>
/// Service to render Blazor components to static HTML strings
/// </summary>
public class HtmlRendererService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public HtmlRendererService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Renders a Blazor component to an HTML string
    /// </summary>
    /// <typeparam name="TComponent">The component type to render</typeparam>
    /// <param name="parameters">Optional parameters to pass to the component</param>
    /// <returns>The rendered HTML as a string</returns>
    public async Task<string> RenderComponentAsync<TComponent>(Dictionary<string, object?>? parameters = null)
        where TComponent : IComponent
    {
        await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _loggerFactory);

        var parameterView = parameters != null
            ? ParameterView.FromDictionary(parameters)
            : ParameterView.Empty;

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync<TComponent>(parameterView);
            return output.ToHtmlString();
        });

        return html;
    }

    /// <summary>
    /// Renders a Blazor component to an HTML string with strongly-typed parameters
    /// </summary>
    /// <typeparam name="TComponent">The component type to render</typeparam>
    /// <param name="parametersBuilder">Action to configure component parameters</param>
    /// <returns>The rendered HTML as a string</returns>
    public async Task<string> RenderComponentAsync<TComponent>(Action<ParameterBuilder<TComponent>> parametersBuilder)
        where TComponent : IComponent
    {
        await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _loggerFactory);

        var builder = new ParameterBuilder<TComponent>();
        parametersBuilder(builder);
        var parameters = builder.Build();

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync<TComponent>(parameters);
            return output.ToHtmlString();
        });

        return html;
    }
}

/// <summary>
/// Helper class to build component parameters in a strongly-typed way
/// </summary>
public class ParameterBuilder<TComponent> where TComponent : IComponent
{
    private readonly Dictionary<string, object?> _parameters = new();

    public ParameterBuilder<TComponent> Add(string name, object? value)
    {
        _parameters[name] = value;
        return this;
    }

    internal ParameterView Build()
    {
        return ParameterView.FromDictionary(_parameters);
    }
}
