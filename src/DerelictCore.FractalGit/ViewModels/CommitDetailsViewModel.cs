using CommunityToolkit.Mvvm.ComponentModel;
using DerelictCore.FractalGit.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DerelictCore.FractalGit.ViewModels;

public partial class CommitDetailsViewModel : ViewModelBase
{
    [ObservableProperty]
    private GitLogLine _line;

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

    // Get via `git show --format=format:%P cd6506c63b0041cc4fe43df4a14d6eb0c3217674`, space-separated long format.
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
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        try
        {
            if (Main is not null && e.PropertyName is nameof(Line))
            {
                await UpdateDetailsAsync();
            }
        }
        catch
        {
            // Ignore errors in async event handling.
        }
    }

    private async Task UpdateDetailsAsync()
    {
        if (Main?.Graph?.WorkingDirectory?.Trim() is not { Length: > 0 } cwd) return;


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
