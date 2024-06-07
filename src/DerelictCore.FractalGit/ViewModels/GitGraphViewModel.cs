using DerelictCore.FractalGit.Models;
using System.Collections.ObjectModel;

namespace DerelictCore.FractalGit.ViewModels;

public class GitGraphViewModel : ViewModelBase
{
    public string WorkingDirectory { get; set; } = "/home/sarah/Projects/DerelictCore.FractalGit";

    public ObservableCollection<GitLogLine> Lines { get; set; } = [];
}
