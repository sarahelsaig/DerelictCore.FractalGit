using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace DerelictCore.FractalGit.Abstractions.Services;

/// <summary>
/// Represents startup instructions for a loaded plugin.
/// </summary>
public interface IStartup
{
    /// <summary>
    /// Gets the plugin's technical name. This is used for declaring it as dependency. It may also be used as key to
    /// declare a friendly display name via localization.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets an optional list of plugins or lifecycle targets which must be loaded and initialized before this plugin
    /// can be loaded.
    /// </summary>
    /// <remarks><para>
    /// Never implement this with in a way that gets <see langword="null"/> or an empty collection. If your plugin does
    /// not have a dependency, attach it to one of the lifecycle targets in <see cref="Startups"/>.
    /// </para></remarks>
    public IEnumerable<string> Dependencies => [nameof(Startups.Setup)];

    /// <summary>
    /// Updates the provided <paramref name="services"/> with ones relevant to this plugin.
    /// </summary>
    public void ConfigureServices(IServiceCollection services);
}
