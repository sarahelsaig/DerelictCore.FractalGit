using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using DerelictCore.FractalGit.Models;
using DerelictCore.FractalGit.Services;
using DerelictCore.FractalGit.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace DerelictCore.FractalGit;

public partial class App : Application
{
    private IServiceScope? _applicationServiceScope;

    public IServiceProvider ApplicationServices =>
        _applicationServiceScope?.ServiceProvider ?? Program.ProgramServices;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        // If you use CommunityToolkit, line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        // Initialize this service provider.
        _applicationServiceScope = Program.ProgramServices.CreateScope();

        var applicationLoadedHandlers = ApplicationServices.GetServices<IApplicationLoadedHandler>().AsList();
        applicationLoadedHandlers.ForEach(handler => handler.BeforeDataContextAttached(this));

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = ApplicationServices.GetRequiredService<MainViewModelAccessor>().ViewModel,
            };
        }
        else if (ApplicationLifetime is not null)
        {
            throw new InvalidOperationException(
                $"Unsupported application lifetime ({ApplicationLifetime?.GetType().FullName ?? "null"}).");
        }

        applicationLoadedHandlers.ForEach(handler => handler.AfterDataContextAttached(this));

        base.OnFrameworkInitializationCompleted();
    }
}
