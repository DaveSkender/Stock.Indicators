namespace Tests.Data;

/// <summary>
/// ULP (Units in Last Place) precision profiles for native numerical type precision comparisons.
/// These control the maximum allowed ULPs when comparing floating-point values based on their
/// native significant digit precision (e.g., ~15-17 significant digits for double).
/// </summary>
/// <remarks>
/// Applies to <see cref="Utilities.AssertEqualsUlp{T}(IEnumerable{T}, IEnumerable{T}, UlpPrecision)"/>
/// <para>
/// ULP-based comparison is more mathematically sound than absolute tolerance for floating-point
/// comparisons as it accounts for the varying precision across the range of representable values.
/// </para>
/// </remarks>
internal enum UlpPrecision
{
    /// <summary>
    /// Exact bit-level equality; no ULP tolerance (difference must be 0 ULPs).
    /// Equivalent to exact equality comparison.
    /// </summary>
    Exact,

    /// <summary>
    /// Allow 1 ULP difference - the smallest possible floating-point step.
    /// Appropriate for values that should be nearly identical but may differ
    /// due to last-bit rounding in calculations.
    /// </summary>
    SingleStep,

    /// <summary>
    /// Allow up to 4 ULPs difference.
    /// Suitable for calculations where small accumulated rounding errors are expected
    /// but the values should still be very close.
    /// </summary>
    NearExact,

    /// <summary>
    /// Allow up to 16 ULPs difference.
    /// More forgiving for complex calculations with multiple intermediate steps
    /// where some precision loss is acceptable.
    /// </summary>
    Standard,

    /// <summary>
    /// Allow up to 64 ULPs difference.
    /// Use with caution; intended for cases where larger floating-point variance
    /// is acceptable due to algorithmic differences or cross-platform variations.
    /// </summary>
    Loose
}
