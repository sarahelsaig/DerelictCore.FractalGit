using CommunityToolkit.Mvvm.ComponentModel;
using DerelictCore.FractalGit.Models;
using System;
using System.Collections.ObjectModel;

namespace DerelictCore.FractalGit.ViewModels;

public partial class CommitDetailsViewModel : ViewModelBase
{
    // Get via `git show --format=format:%B cd6506c63b0041cc4fe43df4a14d6eb0c3217674`.
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

    public GitLogLine Line { get; set; }

    // Get via `git show --format=format:%P cd6506c63b0041cc4fe43df4a14d6eb0c3217674`, space-separated long format.
    public ObservableCollection<string> ParentHashes { get; set; } = [];

    // Commands to check:
    // git branch -a --contains cd6506c63b0041cc4fe43df4a14d6eb0c3217674
    // git name-rev cd6506c63b0041cc4fe43df4a14d6eb0c3217674
    // git name-rev --name-only --exclude=tags/* $SHA
    // git log c0118fa..HEAD --ancestry-path --merges --oneline --color | tail -n 1
    public ObservableCollection<string> ContainingBranches { get; set; } = [];

    public CommitDetailsViewModel() => Line = new();
    public CommitDetailsViewModel(GitLogLine line) => Line = line;
}

public class DesignCommitDetailsViewModel : CommitDetailsViewModel
{
    public DesignCommitDetailsViewModel()
        : base(new()
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
