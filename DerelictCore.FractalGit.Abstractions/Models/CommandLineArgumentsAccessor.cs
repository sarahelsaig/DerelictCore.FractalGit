using System.Collections.Generic;

namespace DerelictCore.FractalGit.Models;

public class CommandLineArgumentsAccessor
{
    public IList<string> Arguments { get; } = [];

    public string? GetSwitch(string name)
    {
        var index = Arguments.IndexOf("--" + name);
        return index < 0 || index + 1 == Arguments.Count
            ? null
            : Arguments[index + 1];
    }
}
