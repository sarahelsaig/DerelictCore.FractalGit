using CommunityToolkit.Mvvm.ComponentModel;
using DerelictCore.FractalGit.Models;

namespace DerelictCore.FractalGit.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _filter = string.Empty;

    [ObservableProperty]
    private int _changedFileCount;

    [ObservableProperty]
    private GitLogLine? _selectedGitLogLine;

    [ObservableProperty]
    private GitGraphViewModel _graph = new();

    [ObservableProperty]
    private CommitDetailsViewModel _details = new();
}
