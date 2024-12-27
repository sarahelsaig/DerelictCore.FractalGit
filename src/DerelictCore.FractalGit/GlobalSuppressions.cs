using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Critical Code Smell",
    "S2302:\"nameof\" should be used",
    Justification = "Too many false positives!",
    Scope = "module")]

[assembly: SuppressMessage(
    "Minor Code Smell",
    "S2333:Redundant modifiers should not be used",
    Justification = "Needed by Avalonia generated code.",
    Scope = "module")]

[assembly: SuppressMessage(
    "Usage",
    "CA2227:Collection properties should be read only",
    Justification = "Should be avoided in Avalonia ViewModels.",
    Scope = "module")]

