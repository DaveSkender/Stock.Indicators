namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the style of an indicator.
/// </summary>
/// <remarks>
/// Values may be hardcoded in the catalog
/// generator and should not be changed.
/// </remarks>
internal enum Style
{
    Series = 0,
    Buffer = 1,
    Stream = 2
}
