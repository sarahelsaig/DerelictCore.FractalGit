using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Critical Code Smell",
    "S2302:\"nameof\" should be used",
    Justification = "Too many false positives!",
    Scope = "module")]

