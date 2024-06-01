using DerelictCore.FractalGit.Models;
using System.Collections.ObjectModel;

namespace DerelictCore.FractalGit.ViewModels;

public class GitGraphViewModel : ViewModelBase
{
    public ObservableCollection<GitLogLine> Lines { get; set; } = [];
}
