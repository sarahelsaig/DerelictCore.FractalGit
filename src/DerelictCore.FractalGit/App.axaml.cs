using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using DerelictCore.FractalGit.JsonConverters;
using DerelictCore.FractalGit.ViewModels;
using DerelictCore.FractalGit.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PalettesDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string,
    Avalonia.Themes.Fluent.ColorPaletteResources>>;

namespace DerelictCore.FractalGit;

public partial class App : Application
{
    private const string PalettesFileName = "palettes.json";
    private static readonly JsonSerializerOptions _paletteJsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = false,
        Converters = { new ColorJsonConverter() },
    };

    public string? CreatePalettesFileWithThemeName { get; set; }
    public string? UsePalette { get; set; } = "Lavender";

    private IDictionary<ThemeVariant, ColorPaletteResources> Palettes => Styles
        .CastWhere<FluentTheme>()
        .First()
        .Palettes;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var palettes = new PalettesDictionary(StringComparer.OrdinalIgnoreCase);

        if (File.Exists(PalettesFileName))
        {
            var json = File.ReadAllText(PalettesFileName);
            palettes.AddRange(JsonSerializer.Deserialize<PalettesDictionary>(json, _paletteJsonOptions) ?? []);
            LoadPalette(palettes);
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        SavePalette(palettes);

        base.OnFrameworkInitializationCompleted();
    }

    private void LoadPalette(PalettesDictionary palettes)
    {
        if (string.IsNullOrWhiteSpace(UsePalette)) return;

        if (!palettes.TryGetValue(UsePalette, out var palette))
        {
            throw new InvalidOperationException($"Could not find palette \"{UsePalette}\". The available palettes " +
                                                $"are {string.Join(", ", palettes.Keys)}.");
        }

        if (palette.TryGetValue(ThemeVariant.Light.Key.ToString()!, out var light)) Palettes[ThemeVariant.Light] = light;
        if (palette.TryGetValue(ThemeVariant.Dark.Key.ToString()!, out var dark)) Palettes[ThemeVariant.Dark] = dark;
    }

    private void SavePalette(PalettesDictionary palettes)
    {
        if (CreatePalettesFileWithThemeName == null) return;

        palettes[CreatePalettesFileWithThemeName] = Styles
            .CastWhere<FluentTheme>()
            .First()
            .Palettes
            .ToDictionary(pair => pair.Key.Key.ToString() ?? string.Empty, pair => pair.Value);

        File.WriteAllText(PalettesFileName, JsonSerializer.Serialize(palettes, palettes.GetType(), _paletteJsonOptions));
    }

    public static App InitApp(IList<string> arguments)
    {
        var app = new App();

        var usePaletteIndex = arguments.IndexOf("--palette");
        if (usePaletteIndex < 0) usePaletteIndex = arguments.IndexOf("--theme");
        if (usePaletteIndex >= 0)
        {
            app.UsePalette = arguments[usePaletteIndex + 1];
        }

        return app;
    }
}
