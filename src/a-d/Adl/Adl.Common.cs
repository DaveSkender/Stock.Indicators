namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (COMMON)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Adl/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public partial class Adl
{
    // increment calculation
    /// <include file='./info.xml' path='info/type[@name="increment"]/*' />
    ///
    public static (double mfm, double mfv, double adl) Increment(
        double lastAdl,
        double high,
        double low,
        double close,
        double volume)
    {
        double mfm = (high == low) ? 0 : (close - low - (high - close)) / (high - low);
        double mfv = mfm * volume;
        double adl = mfv + lastAdl;

        return (mfm, mfv, adl);
    }
}
