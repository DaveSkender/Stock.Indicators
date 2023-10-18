namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (COMMON)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Adl/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public partial class Adl
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

    /// <include file='./info.xml' path='info/type[@name="increment-quote"]/*' />
    ///
    public AdlResult Increment<TQuote>(
        TQuote quote)
        where TQuote : IQuote
    {
        QuoteD q = quote.ToQuoteD();
        AdlResult r = Increment(PrevAdl, q.High, q.Low, q.Close, q.Volume);
        r.Date = q.Date;

        ProtectedResults.Add(r);
        PrevAdl = r.Adl;
        return r;
    }
}
