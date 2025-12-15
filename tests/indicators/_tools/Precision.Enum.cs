namespace Tests.Tools;

// TODO: is Precision enum used or useful?

/// <summary>
/// Precision profiles for numeric comparisons inside <c>BeEquivalentTo(..)</c>.
/// These control the allowed absolute delta when comparing nested numeric members during structural equivalence.
/// </summary>
/// <remarks>
/// Applies to <see cref="TestAsserts.AssertEquals{T}(IEnumerable{T}, IEnumerable{T}, Precision)"/>
/// </remarks>
public enum Precision
{
    /// <summary>
    /// No tolerance; numeric members must match exactly (difference must be 0).
    /// </summary>
    /// <remarks>
    /// Prefer
    /// </remarks>
    Strict,

    /// <summary>
    /// Allow a tiny wobble intended to absorb last-digit differences commonly
    /// seen across runtimes/platforms for double-precision calculations
    /// (e.g., 262.18636765371303 vs 262.1863676537131).
    /// Applies only to floating point types; decimals remain <see cref="Strict"/>.
    /// </summary>
    LastDigit,

    /// <summary>
    /// A bit more lenient than <see cref="LastDigit"/> for scenarios where small rounding accumulations occur.
    /// </summary>
    Tolerant,

    /// <summary>
    /// Use with caution; intended for rare cases where larger cross-platform FP variance is acceptable.
    /// </summary>
    Loose
}
