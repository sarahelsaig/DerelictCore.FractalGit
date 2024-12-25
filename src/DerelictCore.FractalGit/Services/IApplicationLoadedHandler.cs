using Avalonia;

namespace DerelictCore.FractalGit.Services;

/// <summary>
/// A service for handling initialization of application business logic after it has loaded.
/// </summary>
public interface IApplicationLoadedHandler
{
    /// <summary>
    /// Invoked right before the view-model is attached to <see cref="Application.DataContext"/>. This way even
    /// non-reactive properties and fields can be updated.
    /// </summary>
    void BeforeDataContextAttached(Application application);

    /// <summary>
    /// Invoked after the view-model is attached to <see cref="Application.DataContext"/>. At this time it is advisable
    /// to not alter the view-model any further.
    /// </summary>
    void AfterDataContextAttached(Application application);
}
