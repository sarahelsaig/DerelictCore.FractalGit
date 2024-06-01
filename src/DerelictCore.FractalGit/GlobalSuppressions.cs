using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Minor Code Smell",
    "S2333:Redundant modifiers should not be used",
    Justification = "Avalonia files are conventionally partial.",
    Scope = "module")]

[assembly: SuppressMessage(
    "Minor Code Smell",
    "S3220:Method calls should not resolve ambiguously to overloads with \"params\"",
    Justification = "Too many false positives.",
    Scope = "module")]

[assembly: SuppressMessage(
    "Usage",
    "CA2227:Collection properties should be read only",
    Justification = "Not suitable for this type of application.",
    Scope = "module")]
