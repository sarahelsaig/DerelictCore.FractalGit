using Avalonia;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DerelictCore.FractalGit;

public static class Program
{
    private static IList<string> Arguments { get; set; } = [];

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Arguments = args;
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    [SuppressMessage(
        "ReSharper",
        "MemberCanBePrivate.Global",
        Justification = "Avalonia configuration, don't remove; also used by visual designer.")]
    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current
            .Register<FontAwesomeIconProvider>();

        return AppBuilder.Configure(() => App.InitApp(Arguments))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
