namespace DerelictCore.FractalGit.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public GitGraphViewModel Graph { get; set; } = new();
}
