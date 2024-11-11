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
    /// <summary>
    /// Returns the absolute value of a nullable double.
    /// </summary>
    /// <param name="value">The nullable double value.</param>
    /// <returns>The absolute value, or null if the input is null.</returns>
    public static double? Abs(this double? value)
        => value is null
        ? null
        : value < 0 ? (double)-value : (double)value;

    /// <summary>
    /// Rounds a nullable decimal value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The nullable decimal value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value, or null if the input is null.</returns>
    public static decimal? Round(this decimal? value, int digits)
        => value is null
        ? null
        : Math.Round((decimal)value, digits);

    /// <summary>
    /// Rounds a nullable double value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The nullable double value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value, or null if the input is null.</returns>
    public static double? Round(this double? value, int digits)
        => value is null
        ? null
        : Math.Round((double)value, digits);

    /// <summary>
    /// Rounds a double value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The double value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value.</returns>
    public static double Round(this double value, int digits)
        => Math.Round(value, digits);

    /// <summary>
    /// Rounds a decimal value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value.</returns>
    public static decimal Round(this decimal value, int digits)
        => Math.Round(value, digits);

    /// <summary>
    /// Converts a nullable double value to NaN if it is null.
    /// </summary>
    /// <param name="value">The nullable double value.</param>
    /// <returns>The value, or NaN if the input is null.</returns>
    public static double Null2NaN(this double? value)
        => value ?? double.NaN;

    /// <summary>
    /// Converts a nullable decimal value to NaN if it is null.
    /// </summary>
    /// <param name="value">The nullable decimal value.</param>
    /// <returns>The value as a double, or NaN if the input is null.</returns>
    public static double Null2NaN(this decimal? value)
        => value is null
        ? double.NaN
        : (double)value;

    /// <summary>
    /// Converts a nullable double value to null if it is NaN.
    /// </summary>
    /// <param name="value">The nullable double value.</param>
    /// <returns>The value, or null if the input is NaN.</returns>
    public static double? NaN2Null(this double? value)
        => value is double.NaN
        ? null
        : value;

    /// <summary>
    /// Converts a double value to null if it is NaN.
    /// </summary>
    /// <param name="value">The double value.</param>
    /// <returns>The value, or null if the input is NaN.</returns>
    public static double? NaN2Null(this double value)
        => double.IsNaN(value)
        ? null
        : value;
}
