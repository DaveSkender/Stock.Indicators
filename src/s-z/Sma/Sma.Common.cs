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
    public Act Add<TQuote>(
        TQuote quote)
        where TQuote : IQuote
        => Add((quote.Date, (double)quote.Close));

    public new Act Add((DateTime date, double value) price)  // intentionally hides provider Add
    {
        // add price to supplier
        return TupleSupplier.Add(price);
    }

    // add new TResult to local cache
    internal SmaResult Increment((Act act, DateTime date, double _) value)
    {

        // candidate result
        SmaResult r = new(value.date)
        {
            Sma = Increment(value.date)
        };

        // propogate to observers
        if (HandleInboundResult(value.act, r)
            == Act.DoNothing)
        {
            return r;
        }

        // remove NaN
        r.Sma = r.Sma.NaN2Null();

        // save result
        // TODO: fix scenarios and make generic for TResult, see TupleProvider example
        switch (value.act)
        {
            case Act.AddNew:

                ProtectedResults.Add(r);
                break;

            case Act.AddOld:

                ProtectedResults.Add(r);
                ResetHistory(r.Date);
                break;

            case Act.UpdateLast:

                SmaResult last = ProtectedResults[ProtectedResults.Count - 1];
                last.Sma = r.Sma;
                break;

            case Act.UpdateOld:

                SmaResult? u = GetOld(r.Date);
                if (u != null)
                {
                    u.Sma = r.Sma;
                    ResetHistory(r.Date);
                }
                break;

            case Act.Delete:

                SmaResult? d = GetOld(r.Date);
                if (d != null)
                {
                    throw new NotImplementedException();
                }

                break;
            case Act.DoNothing:
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

    private double Increment(DateTime newDate)
    {
        int i = TupleSupplier.ProtectedTuples
            .FindIndex(x => x.Date == newDate);

        // normal
        if (i >= LookbackPeriods - 1)
        {
            double sum = 0;
            for (int w = i - LookbackPeriods + 1; w <= i; w++)
            {
                sum += TupleSupplier.ProtectedTuples[w].Value;
            }

            return sum / LookbackPeriods;
        }

        // warmup periods
        if (i >= 0)
        {
            return double.NaN;
        }

        // i == -1 when source value not found
        throw new InvalidOperationException("Basis not found.");
    }
}
