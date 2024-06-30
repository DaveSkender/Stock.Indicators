namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (COMMON)

/// <summary>
/// See the <see href="https://dotnet.stockindicators.dev/indicators/Adl/">
/// Stock Indicators for .NET online guide</see> for more information.
/// </summary>
public static partial class Adl
{
    /// <summary>
    /// Get the next incremental Accumulation/Distribution Line(ADL) value.
    /// <para>See <see href="https://dotnet.StockIndicators.dev/indicators/Adl/#content?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see>
    /// for more information.</para>
    /// </summary>
    /// <param name="timestamp">Timestamp</param>
    /// <param name="prevAdl">Last ADL value, from prior period</param>
    /// <param name="high">High price, current period</param>
    /// <param name="low">Low price, current period</param>
    /// <param name="close">Close price, current period</param>
    /// <param name="volume">Volume, current period</param>
    /// <returns>New ADL result value.</returns>
    public static AdlResult Increment(
        DateTime timestamp,
        double prevAdl,
        double high,
        double low,
        double close,
        double volume)
    {
        double mfm = high - low == 0
            ? 0
            : (close - low - (high - close))
            / (high - low);

        double mfv = mfm * volume;
        double adl = mfv + prevAdl;

        return new(
            Timestamp: timestamp,
            Adl: adl,
            MoneyFlowMultiplier: mfm,
            MoneyFlowVolume: mfv);
    }
}
