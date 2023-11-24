namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (COMMON)
// TODO: general updates for streaming contexts

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Adl/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public static class Adl
{
    // increment calculation
    /// <include file='./info.xml' path='info/type[@name="increment"]/*' />
    ///
    public static AdlResult Increment(
        double prevAdl,
        double high,
        double low,
        double close,
        double volume)
    {
        double mfm = (high == low) ? 0 : (close - low - (high - close)) / (high - low);
        double mfv = mfm * volume;
        double adl = mfv + prevAdl;

        return new AdlResult(DateTime.MinValue)
        {
            MoneyFlowMultiplier = mfm,
            MoneyFlowVolume = mfv,
            Adl = adl
        };
    }
}
