using CliWrap;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace DerelictCore.FractalGit.Services;

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

public class GitService : IGitService
{
    private readonly ILogger _logger;

    public string GitExecutablePath { get; }
    public string ClonePath { get; }

    public GitService(string gitExecutablePath, string clonePath, ILogger logger)
    {
        _logger = logger;
        GitExecutablePath = gitExecutablePath;
        ClonePath = clonePath;
    }

    public async Task CloneAsync(string repo, params string[] additionalCloneArguments)
    {
        string[] args = ["clone", .. additionalCloneArguments, repo, ClonePath];

        await GitAsync(Path.GetDirectoryName(ClonePath)!, LogDebug, args);
        await GitAsync(ClonePath, LogDebug, "submodule", "update", "--recursive", "--init");
    }

    public Task GitAsync(params string[] arguments) =>
        GitAsync(ClonePath, LogDebug, arguments);

    public async Task<IEnumerable<string>> GitWithOutputAsync(params string[] arguments)
    {
        var lines = new List<string>();
        await GitAsync(ClonePath, lines.Add, arguments);
        return lines;
    }

    private async Task GitAsync(string workingDirectory, Action<string> output, params string[] arguments)
    {
        var gitCommand = $"{GitExecutablePath} {string.Join(' ', arguments)}";
        _logger.LogInformation("Starting command [{GitCommand}] in {WorkingDirectory}...", gitCommand, workingDirectory);

        var builder = Cli.Wrap(GitExecutablePath)
            .WithArguments(arguments)
            .WithWorkingDirectory(workingDirectory)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(output))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(output));

        await builder.ExecuteAsync();

        _logger.LogInformation("Finished command [{GitCommand}] in {WorkingDirectory}.", gitCommand, workingDirectory);
    }

    [SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Not applicable here.")]
    private void LogDebug(string line) => _logger.LogDebug(line);
}
