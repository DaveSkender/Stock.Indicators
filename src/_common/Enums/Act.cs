namespace Skender.Stock.Indicators;

/// <summary>
/// Cache action instruction or outcome.
/// </summary>
internal enum Act
{
    /// <summary>
    /// Adds item to end of cache or rebuild if older.
    /// </summary>
    Add = 0,

    /// <summary>
    /// Does nothing to cache (aborted).
    /// </summary>
    Ignore = 1,

    /// <summary>
    /// Reset and rebuild from marker position.
    /// </summary>
    Rebuild = 2
}
