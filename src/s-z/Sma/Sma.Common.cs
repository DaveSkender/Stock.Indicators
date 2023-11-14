using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (COMMON)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Sma/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public partial class Sma
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

    // INCREMENT CALCULATIONS

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

    // manual use only
    /// <include file='./info.xml' path='info/type[@name="increment-quote"]/*' />
    ///
    public SmaResult Increment<TQuote>(
        TQuote quote)
        where TQuote : IQuote
    {
        // add supplier if missing
        TupleSupplier ??= new TupleProvider();

        // convert to tuple
        (DateTime date, double value) tuple
            = quote.ToTuple(CandlePart.Close);

        // store quote
        TupleSupplier.ProtectedTuples
            .Add(tuple);

        return Increment(tuple);
    }

    // add new TResult to local cache
    // note: different than further storage of Tuple
    internal SmaResult Increment((DateTime date, double value) tp)
    {
        if (TupleSupplier == null)
        {
            throw new ArgumentNullException(
                nameof(TupleSupplier),
                "Could not find data supplier.");
        }

        // initialize
        SmaResult r = new(tp.date);
        int quoteIndex = ProtectedTuples.Count - 1;

        Console.WriteLine($"PRE-RESULTS {ProtectedResults.Count}");

        // first
        if (quoteIndex < LookbackPeriods - 1)
        {
            AddToTupleProvider(r);
            ProtectedResults.Add(r);
            Console.WriteLine($"WRM-RESULTS {ProtectedResults.Count}");
            return r;
        }

        // check against last entry
        SmaResult last = ProtectedResults[ProtectedResults.Count - 1];

        List<(DateTime _, double value)> quotes = TupleSupplier.ProtectedTuples;
        double sma = Increment(quotes, LookbackPeriods, quoteIndex);

        // newer
        if (r.Date > last.Date)
        {
            r.Sma = sma;

            AddToTupleProvider(r);
            ProtectedResults.Add(r);
            Console.WriteLine($"NEW-RESULTS {ProtectedResults.Count}");
            return r;
        }

        // current
        else if (r.Date == last.Date)
        {
            last.Sma = sma;
            AddToTupleProvider(r);
            Console.WriteLine($"DUP-RESULTS {ProtectedResults.Count}");
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
