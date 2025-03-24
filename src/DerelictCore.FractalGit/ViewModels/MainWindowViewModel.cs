using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Linq;
using GitLogLine = DerelictCore.FractalGit.Abstractions.Models.GitLogLine;

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
    private CommitDetailsViewModel _details;

    public MainWindowViewModel() => _details = new(this);

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(SelectedGitLogLine) && SelectedGitLogLine is { } selected)
        {
            Details = new(this, selected);
        }
        else if (e.PropertyName is nameof(Filter))
        {
            Graph.Filter = Filter;
        }
        else if (e.PropertyName is nameof(Graph) && Graph is { } graph)
        {
            SelectedGitLogLine = graph.FilteredLines.First();
        }

        base.OnPropertyChanged(e);
    }
}
