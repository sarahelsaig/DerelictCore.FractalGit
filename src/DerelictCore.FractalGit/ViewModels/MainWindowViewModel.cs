namespace DerelictCore.FractalGit.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Filter { get; set; } = string.Empty;
    public GitGraphViewModel Graph { get; set; } = new();
    public CommitDetailsViewModel Details { get; set; } = new();
    public int ChangedFileCount { get; set; }
}
