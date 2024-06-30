using CommunityToolkit.Mvvm.ComponentModel;
using DerelictCore.FractalGit.Models;
using System.Collections.ObjectModel;

namespace DerelictCore.FractalGit.ViewModels;

public partial class GitGraphViewModel : ViewModelBase
{
    [ObservableProperty]
    private GitLogLine? _selected;

    public string WorkingDirectory { get; set; } = string.Empty;

    public ObservableCollection<GitLogLine> Lines { get; set; } = GitLogLine.SampleDataSet;
}
