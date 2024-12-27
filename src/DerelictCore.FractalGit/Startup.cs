using DerelictCore.FractalGit.Abstractions.Services;
using DerelictCore.FractalGit.Models;
using DerelictCore.FractalGit.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DerelictCore.FractalGit;

public class Startup : IStartup
{
    public string Name => typeof(Program).FullName!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<MainViewModelAccessor>();
        services.AddScoped<IApplicationLoadedHandler, ThemePaletteApplicationLoadedHandler>();
    }
}
