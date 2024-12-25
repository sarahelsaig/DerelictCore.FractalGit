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

        public void ConfigureServices(IServiceCollection services) { }
    }

    public class AfterSetup : IStartup
    {
        public string Name => nameof(AfterSetup);

        public IEnumerable<string> Dependencies { get; } = [nameof(Setup)];

        public void ConfigureServices(IServiceCollection services) { }
    }

    /// <summary>
    /// Creates a new <see cref="ServiceCollection"/> using any <see cref="IStartup"/> implementations found in
    /// assemblies in the <c>plugins</c> directory, loaded in the current assembly, or in any of the <paramref
    /// name="includedAssemblies"/>.
    /// </summary>
    public static ServiceCollection ConfigureServices(IEnumerable<Assembly> includedAssemblies)
    {
        var pluginsPath = Path.Join(
            Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? typeof(Startups).Assembly).Location) ??
                Environment.CurrentDirectory,
            "plugins");

        if (!Directory.Exists(pluginsPath)) Directory.CreateDirectory(pluginsPath);

        var assemblies = Directory.GetFiles(pluginsPath, "*.dll")
            .Select(Assembly.LoadFrom)
            .Concat(AppDomain.CurrentDomain.GetAssemblies())
            .Concat(includedAssemblies)
            .Distinct();

        var startupDictionary = assemblies
            .SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => typeof(IStartup).IsAssignableFrom(type) && type.GetConstructor([]) is { })
            .Distinct()
            .SelectWhere(
                type => type.GetConstructor([])?.Invoke([]) as IStartup,
                startup => !string.IsNullOrEmpty(startup.Name))
            .ToDictionary(startup => startup.Name);
        var sortedStartups = startupDictionary
            .Values
            .OrderTopologicallyBy(startup => startup.Dependencies.Select(name => startupDictionary[name]))
            .ThenBy(startup => startup.GetType().FullName);

        var services = new ServiceCollection();
        foreach (var startup in sortedStartups)
        {
            startup.ConfigureServices(services);
        }

        return services;
    }
}
