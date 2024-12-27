using DerelictCore.FractalGit.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace DerelictCore.FractalGit.Abstractions.Models;

public sealed class ServiceScopeProvider : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public ServiceScopeProvider(ServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public IServiceScope CreateScope()
    {
        var scope = _serviceProvider.CreateScope();

        scope.ServiceProvider
            .GetServices<IStartup>()
            .ForEach(startup => startup.ConfigureScoped(scope.ServiceProvider));

        return scope;
    }

    public void Dispose() => _serviceProvider.Dispose();
}
