using Avalonia;
using DerelictCore.FractalGit.Abstractions.Models;
using DerelictCore.FractalGit.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DerelictCore.FractalGit;

public static class Program
{
    public static ServiceScopeProvider ServiceScopeProvider { get; private set; } =
        new(new ServiceCollection().BuildServiceProvider()); // Temporary, only needed for XAML previews.

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main()
    {
        using var provider = Startups.CreateServiceProvider([typeof(Startups).Assembly, typeof(Program).Assembly]);
        ServiceScopeProvider = provider;

        BuildAvaloniaApp().StartWithClassicDesktopLifetime([]);
    }

    [SuppressMessage(
        "ReSharper",
        "MemberCanBePrivate.Global",
        Justification = "Avalonia configuration, don't remove; also used by visual designer.")]
    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current
            .Register<FontAwesomeIconProvider>();

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
