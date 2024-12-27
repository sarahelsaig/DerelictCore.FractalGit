using CommunityToolkit.Mvvm.ComponentModel;
using DerelictCore.FractalGit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DerelictCore.FractalGit.ViewModels;

public partial class GitGraphViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _filter = string.Empty;

    [ObservableProperty]
    private GitLogLine? _selected;

    public string WorkingDirectory { get; set; } = "/home/sarah/Projects/Open-Source-Orchard-Core-Extensions";

    public ObservableCollection<GitLogLine> Lines { get; set; } = GitLogLine.SampleDataSet;

    public IEnumerable<GitLogLine> FilteredLines
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Filter)) return Lines;

            var words = Filter.Split();
            return Lines
                .Select(line => new
                {
                    Key = $"{line.Hash} {line.Subject} {line.AuthorName} {line.AuthorEmail} {string.Join(' ', line.RefNames)}",
                    Line = line,
                })
                .Where(pair => words.TrueForAll(pair.Key.ContainsOrdinalIgnoreCase))
                .Select(pair => pair.Line);
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Lines) or nameof(Filter))
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(FilteredLines)));
        }

        base.OnPropertyChanged(e);
    }
}
