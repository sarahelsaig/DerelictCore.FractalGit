using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Utilities;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace DerelictCore.FractalGit;

public static class Converters
{
    public static readonly IValueConverter HumanDateTime = new FuncValueConverter<DateTime?, string>(dateTime =>
        dateTime.HasValue ? dateTime.Value.Humanize() : string.Empty);

    public static readonly IValueConverter IsoDateTime = new FuncValueConverter<DateTime?, string>(dateTime =>
        dateTime.HasValue ? dateTime.Value.ToString("o", CultureInfo.InvariantCulture) : string.Empty);

    public static readonly IValueConverter SingleItem = new FuncValueConverter<string?, string[]>(text => [text ?? string.Empty]);

    public static readonly IValueConverter StringTake = new FuncParamValueConverter<string?, int, string>((input, chars) =>
        string.Join(string.Empty, input?.TrimStart().Take(chars) ?? []));

    public static readonly IValueConverter RoslynExpression = new RoslynExpressionConverter();
}

public sealed class FuncParamValueConverter<TIn, TParam, TOut> : IValueConverter
    where TParam : IParsable<TParam>
{
    private readonly Func<TIn?, TParam, TOut> _convert;

    public FuncParamValueConverter(Func<TIn?, TParam, TOut> convert) => _convert = convert;

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is null || !TypeUtilities.CanCast<TIn>(value)) return AvaloniaProperty.UnsetValue;

        TParam param;
        if (TypeUtilities.CanCast<TParam>(parameter))
        {
            param = (TParam)parameter;
        }
        else if (TParam.TryParse(parameter.ToString(), CultureInfo.InvariantCulture, out var result))
        {
            param = result;
        }
        else
        {
            return AvaloniaProperty.UnsetValue;
        }

        return _convert((TIn?)value, param);
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public sealed class RoslynExpressionConverter : IValueConverter
{
    private static readonly ScriptOptions _options = ScriptOptions.Default.AddReferences(typeof(RoslynExpressionConverter).Assembly);

    [SuppressMessage("Usage", "VSTHRD104:Offer async methods", Justification = "Not possible in this context.")]
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter?.ToString() is not { Length: > 0 } expression) return AvaloniaProperty.UnsetValue;

        var code = @$"
            var value = objectValue as {value.GetType().FullName};
            return {expression};
        ";

        return CSharpScript.EvaluateAsync(code, _options, new Context(value, targetType, culture)).Result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();

    public record Context(object? objectValue, Type targetType, CultureInfo culture);
}
