using Avalonia.Media;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DerelictCore.FractalGit.JsonConverters;

public class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);
        var value = node?.GetValue<string?>()?.Trim().TrimStart('#') ?? string.Empty;

        return value.Length switch
        {
            0 => Colors.Transparent,
            3 => Color.FromRgb(Parse(value[0]), Parse(value[1]), Parse(value[2])),
            6 => Color.FromRgb(Parse(value[..2]), Parse(value[2..4]), Parse(value[4..6])),
            8 => Color.FromArgb(Parse(value[..2]), Parse(value[2..4]), Parse(value[4..6]), Parse(value[6..8])),
            _ => throw new InvalidOperationException(
                $"Couldn't parse the color \"{node}\". Expected string with formats \"AARRGGBB\", \"RRGGBB\", " +
                $"\"RGB\" (using hexadecimal digits), or the same with an optional leading pound character."),
        };
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        var text = value.A == byte.MaxValue
            ? Convert.ToHexString([value.R, value.G, value.B])
            : Convert.ToHexString([value.A, value.R, value.G, value.B]);

        if (value == Colors.Transparent) text = string.Empty;

        writer.WriteStringValue(text);
    }

    private static byte Parse(char hex) => Parse($"{hex}{hex}");

    private static byte Parse(string hex) =>
        byte.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
}
