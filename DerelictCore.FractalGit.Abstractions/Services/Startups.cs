using DerelictCore.FractalGit.Abstractions.Models;
using DerelictCore.FractalGit.Models;
using Medallion.Collections;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DerelictCore.FractalGit.Abstractions.Services;

/// <summary>
/// A container class for various lifecycle targets.
/// </summary>
public static class Startups
{
    public class BeforeSetup : IStartup
    {
        public string Name => nameof(BeforeSetup);

        // This is the one exception to the rule of not having empty dependencies, making this target the universal
        // starting point.
        public IEnumerable<string> Dependencies { get; } = [];

        public void ConfigureServices(IServiceCollection services) { }
    }

    public class Setup : IStartup
    {
        public string Name => nameof(Setup);

        public IEnumerable<string> Dependencies { get; } = [nameof(BeforeSetup)];

        public void ConfigureServices(IServiceCollection services) =>
            services.AddSingleton<CommandLineArgumentsAccessor>();

        public void ConfigureSingleton(IServiceProvider serviceProvider) =>
            serviceProvider.GetRequiredService<CommandLineArgumentsAccessor>().Arguments.SetItems(
                Environment.GetCommandLineArgs()[1..]);
    }

    public class AfterSetup : IStartup
    {
        public string Name => nameof(AfterSetup);

        public IEnumerable<string> Dependencies { get; } = [nameof(Setup)];

        public void ConfigureServices(IServiceCollection services) { }
    }

    /// <summary>
    /// Creates a new <see cref="ServiceScopeProvider"/> using any <see cref="IStartup"/> implementations found by the
    /// <c>./plugins/*.dll</c> and <c>./*.Plugin.dll</c> paths, loaded in the current assembly, or in any of the
    /// <paramref name="includedAssemblies"/>. If there is a <see cref="IStartup.Name"/> clash, the startup is selected
    /// to be kept in the above order of precedence.
    /// </summary>
    public static ServiceScopeProvider CreateServiceProvider(IEnumerable<Assembly> includedAssemblies)
    {
        var root = Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? typeof(Startups).Assembly).Location) ??
                   Environment.CurrentDirectory;
        var pluginsPath = Path.Join(root, "plugins");

        if (!Directory.Exists(pluginsPath)) Directory.CreateDirectory(pluginsPath);

        var assemblies = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.TopDirectoryOnly)
            .Concat(Directory.GetFiles(root, "*.Plugin.dll", SearchOption.TopDirectoryOnly))
            .Select(Assembly.LoadFrom)
            .Concat(AppDomain.CurrentDomain.GetAssemblies())
            .Concat(includedAssemblies);

        var startupDictionary = assemblies
            .SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => typeof(IStartup).IsAssignableFrom(type) && type.GetConstructor([]) is { })
            .SelectWhere(type => type.GetConstructor([])?.Invoke([]) as IStartup)
            .Where(startup => !string.IsNullOrEmpty(startup.Name))
            .DistinctBy(startup => startup.Name)
            .ToDictionary(startup => startup.Name);
        var sortedStartups = startupDictionary
            .Values
            .OrderTopologicallyBy(startup => startup.Dependencies.Select(name => startupDictionary[name]))
            .ThenBy(startup => startup.GetType().FullName)
            .ToList();

        var services = new ServiceCollection();
        services.AddSingleton<IEnumerable<IStartup>>(sortedStartups);
        foreach (var startup in sortedStartups)
        {
            startup.ConfigureServices(services);
        }

        var provider = services.BuildServiceProvider();
        foreach (var startup in sortedStartups)
        {
            startup.ConfigureSingleton(provider);
        }

        return new(provider);
    }
}
