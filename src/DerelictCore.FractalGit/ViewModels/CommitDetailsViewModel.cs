using DerelictCore.FractalGit.Models;
using System;

namespace DerelictCore.FractalGit.ViewModels;

public class CommitDetailsViewModel : ViewModelBase
{
    public GitLogLine Line { get; set; } = new();
    public string? Details { get; set; }
}

public class DesignCommitDetailsViewModel : CommitDetailsViewModel
{
    public DesignCommitDetailsViewModel() =>
        Line = new()
        {
            Graph = "| * | | | | | |",
            Hash = "cd6506c63b0041cc4fe43df4a14d6eb0c3217674",
            AuthorName = "Sára El-Saig",
            AuthorEmail = "sara.el-saig@lombiq.com",
            AuthorDate = new DateTime(2024, 6, 6, 11, 3, 58, DateTimeKind.Local),
            RefNames = ["HEAD -\u003E dev", "origin/dev", "origin/HEAD"],
            Subject = "Merge remote-tracking branch 'origin/dev' into issue/OCC-245-cloudsmith",
        };
}
