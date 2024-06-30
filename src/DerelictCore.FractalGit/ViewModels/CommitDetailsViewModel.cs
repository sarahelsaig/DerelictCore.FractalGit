using CommunityToolkit.Mvvm.ComponentModel;
using DerelictCore.FractalGit.Models;
using DerelictCore.FractalGit.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DerelictCore.FractalGit.ViewModels;

public partial class CommitDetailsViewModel : ViewModelBase
{
    [ObservableProperty]
    private GitLogLine _line;

    [ObservableProperty]
    private string? _body;

    // Get via `git symbolic-ref refs/remotes/origin/HEAD`, add converter to trim out 'refs/remotes/origin/' for display.
    [ObservableProperty]
    private string? _defaultBranchName;

    // Get via `git rev-list --left-right --count refs/remotes/origin/dev...cd6506c63b0041cc4fe43df4a14d6eb0c3217674`.
    [ObservableProperty]
    private int _commitsAhead;
    [ObservableProperty]
    private int _commitsBehind;

    public ObservableCollection<string> ParentHashes { get; set; } = [];

    // Commands to check:
    // git branch -a --contains cd6506c63b0041cc4fe43df4a14d6eb0c3217674
    // git name-rev cd6506c63b0041cc4fe43df4a14d6eb0c3217674
    // git name-rev --name-only --exclude=tags/* $SHA
    // git log c0118fa..HEAD --ancestry-path --merges --oneline --color | tail -n 1
    public ObservableCollection<string> ContainingBranches { get; set; } = [];

    public MainWindowViewModel? Main { get; }

    public CommitDetailsViewModel(MainWindowViewModel? main, GitLogLine? line = null)
    {
        Main = main;
        Line = line ?? new();

        // Intentional fire-and-forget call.
#pragma warning disable VSTHRD110
#pragma warning disable MA0134
        UpdateDetailsAsync();
#pragma warning restore MA0134
#pragma warning restore VSTHRD110
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName is nameof(Line))
        {
            await UpdateDetailsAsync();
        }
    }

    private Task UpdateDetailsAsync() =>
        Main is not null && Main?.Graph?.WorkingDirectory?.Trim() is { Length: > 0 } cwd && Line.Hash is { } hash
            ? UpdateDetailsInnerAsync(new GitService("git", cwd, NullLogger.Instance), hash)
            : Task.CompletedTask;

    private async Task UpdateDetailsInnerAsync(IGitService service, string hash)
    {
        try
        {
            Body = string.Join('\n', await service.GitWithOutputAsync("show", "--format=format:%B", hash)).Trim();
            ParentHashes.SetItems((await service.GitWithOutputAsync("show", "--format=format:%P", hash))
                .WhereNot(string.IsNullOrWhiteSpace)
                .FirstOrDefault()?
                .Split()
                .WhereNot(string.IsNullOrEmpty));
        }
        catch
        {
            // Ignore errors in async event handling.
        }
    }
}

public class DesignCommitDetailsViewModel : CommitDetailsViewModel
{
    public DesignCommitDetailsViewModel()
        : base(main: null, line: new()
        {
            Graph = "| * | | | | | |",
            Hash = "cd6506c63b0041cc4fe43df4a14d6eb0c3217674",
            AuthorName = "Sára El-Saig",
            AuthorEmail = "sara.el-saig@lombiq.com",
            AuthorDate = new DateTime(2024, 6, 6, 11, 3, 58, DateTimeKind.Local),
            RefNames = ["HEAD -\u003E dev", "origin/dev", "origin/HEAD"],
            Subject = "Merge remote-tracking branch 'origin/dev' into issue/OCC-245-cloudsmith",
        })
    {
    }
}
