namespace FacioQuo.Stock.Indicators;

/// <summary>
/// A hub's result cache and internal indicator state at one point in time.
/// </summary>
/// <remarks>
/// Restoring a snapshot into a hub with the same name and an aligned provider
/// resumes streaming without replaying the source history.
/// </remarks>
/// <typeparam name="TOut">Type of the hub's results.</typeparam>
/// <param name="FormatVersion">Version of this envelope; a restore rejects any other value.</param>
/// <param name="HubName">Name of the hub that captured the snapshot, such as <c>RSI(14)</c>.</param>
/// <param name="Results">Copy of the hub's result cache.</param>
/// <param name="State">The hub's internal state, in the hub's own versioned encoding.</param>
internal sealed record HubSnapshot<TOut>(
    int FormatVersion,
    string HubName,
    IReadOnlyList<TOut> Results,
    byte[] State);

/// <summary>
/// Version of the <see cref="HubSnapshot{TOut}"/> envelope.
/// </summary>
internal static class HubSnapshot
{
    /// <summary>
    /// Current envelope version.
    /// </summary>
    internal const int FormatVersion = 1;
}
