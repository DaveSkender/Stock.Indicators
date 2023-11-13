using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (COMMON)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Sma/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public partial class Sma : ChainProvider
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }

    // increment calculation
    /// <include file='./info.xml' path='info/type[@name="increment-tuple"]/*' />
    ///
    public static double Increment(
      Collection<(DateTime Date, double Value)> priceList)
    {
        double[] prices = priceList
            .Select(x => x.Value)
            .ToArray();

        return Increment(prices);
    }

    /// <include file='./info.xml' path='info/type[@name="increment-array"]/*' />
    ///
    public static double Increment(
      double[] prices)
    {
        if (prices is null || prices.Length == 0)
        {
            return double.NaN;
        }

        int length = prices.Length;

        double sum = 0;
        for (int i = 0; i < length; i++)
        {
            sum += prices[i];
        }

        return sum / length;
    }

    /// <include file='./info.xml' path='info/type[@name="increment-quote"]/*' />
    ///
    public SmaResult Increment<TQuote>(
        TQuote quote)
        where TQuote : IQuote
    {
        Supplier ??= new TupleProvider();

        QuoteD q = quote.ToQuoteD();

        // store quote
        Supplier.ProtectedTuples
            .Add((q.Date, q.Close));

        return Increment((q.Date, q.Close));
    }

    // add new tuple quote
    internal SmaResult Increment((DateTime date, double value) tp)
    {
        if (Supplier == null)
        {
            throw new ArgumentNullException(
                nameof(Supplier),
                "Could not find data supplier.");
        }

        // initialize
        SmaResult r = new(tp.date);
        int i = ProtectedResults.Count;

        // initialization periods
        if (i < LookbackPeriods - 1)
        {
            ProtectedResults.Add(r);
            SendToChain(r);
            return r;
        }

        // calculate incremental value
        List<(DateTime _, double value)> quotes = Supplier.ProtectedTuples;

        double sma = Increment(quotes, LookbackPeriods, i);

        // check against last entry
        SmaResult last = ProtectedResults[i - 1];

        // add new
        if (r.Date > last.Date)
        {
            r.Sma = sma;

            ProtectedResults.Add(r);
            SendToChain(r);
            return r;
        }

        // update last
        else if (r.Date == last.Date)
        {
            last.Sma = sma;

            SendToChain(last);
            return last;
        }

        // late arrival
        else
        {
            // heal
            throw new NotImplementedException();
        }
    }

    private static double Increment(
        List<(DateTime _, double value)> tpQuotes,
        int lookbackPeriods,
        int index)
    {
        double sum = 0;

        for (int i = index - lookbackPeriods + 1; i <= index; i++)
        {
            sum += tpQuotes[i].value;
        }

        return sum / lookbackPeriods;
    }
}
