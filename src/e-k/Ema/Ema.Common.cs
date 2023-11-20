namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (COMMON)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Ema/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public partial class Ema
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for EMA.");
        }
    }

    // INCREMENT CALCULATIONS

    /// <include file='./info.xml' path='info/type[@name="increment-k"]/*' />
    ///
    public static double Increment(
        double k,
        double lastEma,
        double newPrice)
        => lastEma + (k * (newPrice - lastEma));

    /// <include file='./info.xml' path='info/type[@name="increment-lookback"]/*' />
    ///
    public static double Increment(
        int lookbackPeriods,
        double lastEma,
        double newPrice)
    {
        double k = 2d / (lookbackPeriods + 1);
        return Increment(k, lastEma, newPrice);
    }

    /// <include file='./info.xml' path='info/type[@name="increment-quote"]/*' />
    ///
    public Act Add<TQuote>(TQuote quote)
        where TQuote : IQuote
        => Add((quote.Date, (double)quote.Close));

    public new Act Add((DateTime Date, double Value) price)  // intentionally hides provider Add
=>
        // add price to supplier
        TupleSupplier.Add(price);

    internal EmaResult Increment((Act act, DateTime date, double value) value)
    {
        // initialize
        EmaResult r = new(value.date)
        {
            Ema = Increment(value.date, value.value)
        };

        // propogate to observers
        if (HandleInboundResult(value.act, r)
            == Act.DoNothing)
        {
            return r;
        }

        // remove NaN
        r.Ema = r.Ema.NaN2Null();

        // save result
        // TODO: fix scenarios and make generic for TResult, see TupleProvider example
        switch (value.act)
        {
            case Act.AddNew:

                ProtectedResults.Add(r);
                break;
            case Act.AddOld:

                ProtectedResults.Add(r);
                throw new NotImplementedException();

            case Act.UpdateLast:
                throw new NotImplementedException();

            case Act.UpdateOld:
                throw new NotImplementedException();

            case Act.Delete:
                throw new NotImplementedException();

            case Act.DoNothing:
                // handed above
                break;

            default:
                break;
        }

        return r;
    }

    private double Increment(DateTime date, double newPrice)
    {
        int i = TupleSupplier.ProtectedTuples
            .FindIndex(x => x.Date == date);

        // warmup periods (normal)
        if (i >= 0 && i < LookbackPeriods - 1)
        {
            return double.NaN;
        }

        // normal
        if (!double.IsNaN(ProtectedTuples[i - 1].Value))
        {
            return Increment(K, ProtectedTuples[i - 1].Value, newPrice);
        }

        // set first value (normal) or reset (offset warmup case)
        if (i >= LookbackPeriods - 1 && double.IsNaN(ProtectedTuples[i - 1].Value))
        {
            double sum = 0;
            for (int w = i - LookbackPeriods + 1; w <= i; w++)
            {
                sum += TupleSupplier.ProtectedTuples[w].Value;
            }

            return sum / LookbackPeriods;
        }

        // i == -1 when source value not found
        throw new InvalidOperationException("Basis not found.");
    }
}
