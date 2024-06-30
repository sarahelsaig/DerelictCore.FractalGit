using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DerelictCore.FractalGit.Models;

public partial class GitLogLine
{
    private static ObservableCollection<GitLogLine>? _sampleDataSet;

    public static ObservableCollection<GitLogLine> SampleDataSet
    {
        get
        {
            if (_sampleDataSet == null)
            {
                // Update the GitLogLine.json using the unit test and then copy the new value here.
                var json = File.ReadAllText(Path.Join("Models", "GitLogLine.json"));
                var list = JsonSerializer.Deserialize<IEnumerable<GitLogLine>>(json) ?? [];
                _sampleDataSet = new(list);
            }

            return _sampleDataSet;
        }
    }
}
