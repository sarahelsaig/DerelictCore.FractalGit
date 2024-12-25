using Avalonia;
using Avalonia.Themes.Fluent;
using DerelictCore.FractalGit.JsonConverters;
using DerelictCore.FractalGit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static Avalonia.Styling.ThemeVariant;
using PalettesDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string,
    Avalonia.Themes.Fluent.ColorPaletteResources>>;

namespace DerelictCore.FractalGit.Services;

public class ThemePaletteApplicationLoadedHandler : IApplicationLoadedHandler
{
    private const string PalettesFileName = "palettes.json";

    private readonly CommandLineArgumentsAccessor _claa;
    private readonly PalettesDictionary _palettes = new(StringComparer.OrdinalIgnoreCase);

    public ThemePaletteApplicationLoadedHandler(CommandLineArgumentsAccessor claa) => _claa = claa;

    private static readonly JsonSerializerOptions _paletteJsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = false,
        Converters = { new ColorJsonConverter() },
    };

    public void BeforeDataContextAttached(Application application)
    {
        if (!File.Exists(PalettesFileName)) return;

        var json = File.ReadAllText(PalettesFileName);
        _palettes.AddRange(JsonSerializer.Deserialize<PalettesDictionary>(json, _paletteJsonOptions) ?? []);

        var usePalette = _claa.GetSwitch("palette") ?? _claa.GetSwitch("theme") ?? "Lavender";
        if (string.IsNullOrWhiteSpace(usePalette)) return;

        if (!_palettes.TryGetValue(usePalette, out var palette))
        {
            throw new InvalidOperationException($"Could not find palette \"{usePalette}\". The available palettes " +
                                                $"are {string.Join(", ", _palettes.Keys)}.");
        }

        var appPalettes = application
            .Styles
            .CastWhere<FluentTheme>()
            .First()
            .Palettes;

        if (palette.TryGetValue(Light.Key.ToString()!, out var light)) appPalettes[Light] = light;
        if (palette.TryGetValue(Dark.Key.ToString()!, out var dark)) appPalettes[Dark] = dark;
    }

    public void AfterDataContextAttached(Application application)
    {
        if (_claa.GetSwitch("save-palette") is not { } name)
        {
            return;
        }

        _palettes[name] = application
            .Styles
            .CastWhere<FluentTheme>()
            .First()
            .Palettes
            .ToDictionary(pair => pair.Key.Key.ToString() ?? string.Empty, pair => pair.Value);

        File.WriteAllText(PalettesFileName, JsonSerializer.Serialize(_palettes, _palettes.GetType(), _paletteJsonOptions));
    }
}
