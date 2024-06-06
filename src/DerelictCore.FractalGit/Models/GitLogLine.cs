using CliWrap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace DerelictCore.FractalGit.Models;

public class GitLogLine
{
    public string? Graph { get; set; }
    public string? Hash { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorEmail { get; set; }
    public DateTime? AuthorDate { get; set; }
    public IList<string> RefNames { get; init; } = [];
    public string? Subject { get; set; }

    public static async Task<IEnumerable<GitLogLine>> ExecuteAsync(
        string gitExecutablePath,
        string workingDirectory,
        CancellationToken cancellationToken = default)
    {
        // SEP=";;$(date +%s);;"; git log --all --oneline --graph --no-abbrev-commit \
        //   --format=format:"${SEP}%H${SEP}%aN${SEP}%aE${SEP}%aI${SEP}%D${SEP}%s"

        try
        {
            var separator = $";;{Guid.NewGuid():N};;";
            var arguments = new[]
            {
                "log",
                "--all",
                "--oneline",
                "--graph",
                "--no-abbrev-commit",
                "--format=format:" + string.Join(separator, string.Empty, "%H", "%aN", "%aE", "%aI", "%D", "%s"),
            };

            var results = new List<GitLogLine>();
            await Cli.Wrap(gitExecutablePath)
                .WithArguments(arguments)
                .WithWorkingDirectory(workingDirectory)
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                {
                    if (!string.IsNullOrWhiteSpace(line) && line.Split(separator) is { Length: 7 } x)
                    {
                        results.Add(new GitLogLine
                        {
                            Graph = x[0].TrimEnd(),
                            Hash = x[1],
                            AuthorName = x[2],
                            AuthorEmail = x[3],
                            AuthorDate = DateTime.Parse(x[4], CultureInfo.InvariantCulture),
                            RefNames = x[5].Split(", "),
                            Subject = x[6],
                        });
                    }
                }))
                .ExecuteAsync(cancellationToken);

            return results;
        }
        catch (Exception exception)
        {
            await exception.AlertAsync();
            return [];
        }
    }
}
