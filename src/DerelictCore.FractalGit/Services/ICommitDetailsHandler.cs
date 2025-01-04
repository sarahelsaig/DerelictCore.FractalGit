using Avalonia.Controls;
using DerelictCore.FractalGit.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DerelictCore.FractalGit.Services;

/// <summary>
/// Additional customization for product details.
/// </summary>
public interface ICommitDetailsHandler
{
    /// <summary>
    /// Add one or more new tab with custom content.
    /// </summary>
    /// <returns>A collection of header text - child control pairs.</returns>
    public Task<IEnumerable<KeyValuePair<object, Control>>> AddTabAsync(CommitDetails commitDetails) =>
        Task.FromResult<IEnumerable<KeyValuePair<object, Control>>>([]);
}
