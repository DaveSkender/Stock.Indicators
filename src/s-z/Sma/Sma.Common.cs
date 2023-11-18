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
    public SmaResult Add<TQuote>(
        TQuote quote)
        where TQuote : IQuote
        => Add((quote.Date, (double)quote.Close));

    public new SmaResult Add((DateTime date, double value) price)  // intentionally hides provider Add
    {
        // store price
        TupleSupplier.ProtectedTuples.Add(price);
        return Increment((Disposition.AddNew, price.date, price.value));
    }

    // add new TResult to local cache
    internal SmaResult Increment((Disposition disposition, DateTime date, double _) value)
    {
        // candidate result
        SmaResult r = new(value.date)
        {
            Sma = Increment(
            TupleSupplier.ProtectedTuples,
            LookbackPeriods,
            ProtectedResults.Count)
        };

        // propogate to observers
        if (HandleInboundResult(value.disposition, r)
            == Disposition.DoNothing)
        {
            return r;
        }

        // remove NaN
        r.Sma = r.Sma.NaN2Null();

        // save result
        switch (value.disposition)
        {
            case Disposition.AddNew:

                ProtectedResults.Add(r);
                break;

            case Disposition.AddOld:

                ProtectedResults.Add(r);
                ResetHistory(r.Date);
                break;

            case Disposition.UpdateLast:

                SmaResult last = ProtectedResults[ProtectedResults.Count - 1];
                last.Sma = r.Sma;
                break;

            case Disposition.UpdateOld:

                SmaResult? u = GetOld(r.Date);
                if (u != null)
                {
                    u.Sma = r.Sma;
                    ResetHistory(r.Date);
                }
                break;

            case Disposition.Delete:

                SmaResult? d = GetOld(r.Date);
                if (d != null)
                {
                    throw new NotImplementedException();
                }

                break;
            case Disposition.DoNothing:
                // handled by propogator
                break;
            default:
                throw new InvalidOperationException();
        }

        return r;
    }

    // TODO: refactor these as TResult to TupleProvider?
    // at a minimum, must mirror there.
    private SmaResult? GetOld(DateTime date)
    {
        int i = IndexOld(date);
        return i == -1 ? null : ProtectedResults[i];
    }

    private int IndexOld(DateTime date)
        => ProtectedResults
            .FindIndex(x => x.Date == date);

    private void ResetHistory(DateTime date)
    {
        int i = IndexOld(date);

        if (i == -1)
        {
            return;
        }

        ResetHistory(i);
    }

    private void ResetHistory(int index) => throw new NotImplementedException();

    private static double Increment(
        List<(DateTime _, double value)> tpQuotes,
        int lookbackPeriods,
        int quotesIndex)
    {
        // TODO: add error handling for warmup periods that return
        // TODO: since this is private and accessing own properties,
        // whey even ask for parameters.  This smells bad.  Simplify.

        double sum = 0;

        for (int i = quotesIndex - lookbackPeriods + 1; i <= quotesIndex; i++)
        {
            sum += tpQuotes[i].value;
        }

        return sum / lookbackPeriods;
    }
}
