namespace Skender.Stock.Indicators;

/// <summary>
/// Nullable System.<see cref="Math"/> functions.
/// </summary>
/// <remarks>
/// <c>System.Math</c> infamously does not allow
/// or handle nullable input values.
/// Instead of adding repetitive inline defensive code,
/// we're using these equivalents.  Most are simple wrappers.
/// </remarks>
public static class NullMath
{
    public static double? Abs(this double? value)
        => value is null
        ? null
        : value < 0 ? (double)-value : (double)value;

    public static decimal? Round(this decimal? value, int digits)
        => value is null
        ? null
        : Math.Round((decimal)value, digits);

    public static double? Round(this double? value, int digits)
        => value is null
        ? null
        : Math.Round((double)value, digits);

    public static double Round(this double value, int digits)
        => Math.Round(value, digits);

    public static decimal Round(this decimal value, int digits)
        => Math.Round(value, digits);

    public static double Null2NaN(this double? value)
        => value ?? double.NaN;

    public static double Null2NaN(this decimal? value)
        => value is null
        ? double.NaN
        : (double)value;

    public static double? NaN2Null(this double? value)
        => value is double.NaN
        ? null
        : value;

    public static double? NaN2Null(this double value)
        => double.IsNaN(value)
        ? null
        : value;
}
