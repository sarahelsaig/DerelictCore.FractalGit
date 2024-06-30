using DerelictCore.FractalGit.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DerelictCore.FractalGit.Models;

public partial class GitLogLine
{
    public string? Graph { get; set; }
    public string? Hash { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorEmail { get; set; }
    public DateTime? AuthorDate { get; set; }
    public IList<string> RefNames { get; init; } = [];
    public string? Subject { get; set; }

    public static async Task<IEnumerable<GitLogLine>> ExecuteAsync(
        IGitService gitService,
        CancellationToken cancellationToken = default)
    {
        // SEP=";;$(date +%s);;"; git log --all --oneline --graph --no-abbrev-commit \
        //   --format=format:"${SEP}%H${SEP}%aN${SEP}%aE${SEP}%aI${SEP}%D${SEP}%s"

        try
        {
            var separator = $";;{Guid.NewGuid():N};;";
            var lines = await gitService.GitWithOutputAsync(
                "log",
                "--all",
                "--oneline",
                "--graph",
                "--no-abbrev-commit",
                "--format=format:" + string.Join(separator, string.Empty, "%H", "%aN", "%aE", "%aI", "%D", "%s"));

            var results = lines
                .SelectMany<string, GitLogLine>(line => !string.IsNullOrWhiteSpace(line) && line.Split(separator) is { Length: 7 } x
                    ? [
                        new()
                        {
                            Graph = x[0].TrimEnd(),
                            Hash = x[1],
                            AuthorName = x[2],
                            AuthorEmail = x[3],
                            AuthorDate = DateTime.Parse(x[4], CultureInfo.InvariantCulture),
                            RefNames = x[5].Trim().Split(", ", StringSplitOptions.RemoveEmptyEntries),
                            Subject = x[6],
                        }
                    ]
                    : [])
                .ToList();

            return results;
        }
        catch (Exception exception)
        {
            await exception.AlertAsync();
            return [];
        }
    }
}
