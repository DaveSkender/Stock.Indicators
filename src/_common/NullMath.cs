namespace Skender.Stock.Indicators;

// NULLABLE SYSTEM.MATH
// System.Math does not allow or handle null input values.
// Instead of putting a lot of inline defensive code
// we're building nullable equivalents here.
internal static class NullMath
{
    internal static double? Abs(double? value)
        => (value is null)
        ? null
        : value < 0 ? (double)-value : (double)value;

    internal static decimal? Round(decimal? value, int digits)
        => (value is null)
        ? null
        : Math.Round((decimal)value, digits);

    internal static double? Round(double? value, int digits)
        => (value is null)
        ? null
        : Math.Round((double)value, digits);

    internal static double Null2NaN(this double? value)
        => (value is null)
        ? double.NaN
        : (double)value;

    internal static double? NaN2Null(this double? value)
        => (value is double and double.NaN)
        ? null
        : value;

    internal static double? NaN2Null(this double value)
        => (value is double and double.NaN)
        ? null
        : value;
}
