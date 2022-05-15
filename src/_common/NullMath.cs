namespace Skender.Stock.Indicators;

// NULLABLE SYSTEM.MATH
// System.Math does not allow or handle null input values.
// Instead of putting a lot of inline defensive code
// we're building nullable equivalents here.
internal static class NullMath
{
    internal static decimal? Abs(decimal? value)
        => (value is null)
        ? null
        : value < 0 ? (decimal)-value : (decimal)value;

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
}
