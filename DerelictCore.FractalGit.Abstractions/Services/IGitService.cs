using System.Collections.Generic;
using System.Threading.Tasks;

namespace DerelictCore.FractalGit.Abstractions.Services;

/// <summary>
/// An abstraction over calling git.
/// </summary>
public interface IGitService
{
    /// <summary>
    /// Gets the path of the git executable (<c>.exe</c> file on Windows).
    /// </summary>
    string GitExecutablePath { get; }

    /// <summary>
    /// Gets the path of the local repository.
    /// </summary>
    string ClonePath { get; }

    /// <summary>
    /// Clones the provided <paramref name="repo"/> into <see cref="ClonePath"/>. Assumes that <see cref="ClonePath"/>
    /// does not exist but its parent directory does.
    /// </summary>
    Task CloneAsync(string repo, params string[] additionalCloneArguments);

    /// <summary>
    /// Executes git with the provided <paramref name="arguments"/>.
    /// </summary>
    Task GitAsync(params string[] arguments);

    /// <summary>
    /// Executes git with the provided <paramref name="arguments"/> and returns the standard output and error stream's
    /// lines.
    /// <see cref="string"/>.
    /// </summary>
    Task<IEnumerable<string>> GitWithOutputAsync(params string[] arguments);
}
