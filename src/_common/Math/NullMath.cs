namespace Skender.Stock.Indicators;

// NULLABLE SYSTEM.MATH
// System.Math does not allow or handle null input values.
// Instead of putting a lot of inline defensive code
// we're building nullable equivalents here.
public static class NullMath
{
    public static double? Abs(this double? value)
        => (value is null)
        ? null
        : value < 0 ? (double)-value : (double)value;

    public static decimal? Round(this decimal? value, int digits)
        => (value is null)
        ? null
        : Math.Round((decimal)value, digits);

    public static double? Round(this double? value, int digits)
        => (value is null)
        ? null
        : Math.Round((double)value, digits);

    public static double Round(this double value, int digits)
        => Math.Round(value, digits);

    public static decimal Round(this decimal value, int digits)
        => Math.Round(value, digits);

    public static double Null2NaN(this double? value)
        => (value is null)
        ? double.NaN
        : (double)value;

    public static double? NaN2Null(this double? value)
        => (value is double and double.NaN)
        ? null
        : value;

    public static double? NaN2Null(this double value)
        => (value is double and double.NaN)
        ? null
        : value;
}
