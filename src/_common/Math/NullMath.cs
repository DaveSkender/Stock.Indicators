using System.Runtime.CompilerServices;

namespace Skender.Stock.Indicators;

/// <summary>
/// Nullable <c>System.<see cref="Math"/></c> functions.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? Abs(this double? value)
        => value.HasValue
        ? (value.GetValueOrDefault() < 0
            ? -value.GetValueOrDefault()
            : value)
        : null;

    /// <summary>
    /// Rounds a nullable decimal value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The nullable decimal value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value, or null if the input is null.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal? Round(this decimal? value, int digits)
        => value.HasValue
        ? Math.Round(value.GetValueOrDefault(), digits)
        : null;

    /// <summary>
    /// Rounds a nullable double value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The nullable double value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value, or null if the input is null.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? Round(this double? value, int digits)
        => value.HasValue
        ? Math.Round(value.GetValueOrDefault(), digits)
        : null;

    /// <summary>
    /// Rounds a double value to a specified number of fractional digits.
    /// It is an extension alias of <see cref="Math.Round(double, int)"/>
    /// </summary>
    /// <param name="value">The double value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double value, int digits)
        => Math.Round(value, digits);

    /// <summary>
    /// Rounds a decimal value to a specified number of fractional digits.
    /// It is an extension alias of <see cref="Math.Round(decimal, int)"/>
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(this decimal value, int digits)
        => Math.Round(value, digits);

    /// <summary>
    /// Converts a nullable double value to NaN if it is null.
    /// </summary>
    /// <param name="value">The nullable double value.</param>
    /// <returns>The value, or NaN if the input is null.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Null2NaN(this double? value)
        => value.GetValueOrDefault(double.NaN);

    /// <summary>
    /// Converts a nullable decimal value to NaN if it is null.
    /// </summary>
    /// <param name="value">The nullable decimal value.</param>
    /// <returns>The value as a double, or NaN if the input is null.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Null2NaN(this decimal? value)
        => value.HasValue
        ? (double)value.GetValueOrDefault()
        : double.NaN;

    /// <summary>
    /// Converts a nullable double value to null if it is NaN.
    /// </summary>
    /// <param name="value">The nullable double value.</param>
    /// <returns>The value, or null if the input is NaN.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? NaN2Null(this double? value)
        => value.HasValue && double.IsNaN(value.GetValueOrDefault())
        ? null
        : value;

    /// <summary>
    /// Converts a double value to null if it is NaN.
    /// </summary>
    /// <param name="value">The double value.</param>
    /// <returns>The value, or null if the input is NaN.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? NaN2Null(this double value)
        => double.IsNaN(value)
        ? null
        : value;
}
