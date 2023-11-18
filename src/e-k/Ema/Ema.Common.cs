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
    public EmaResult Add<TQuote>(TQuote quote)
        where TQuote : IQuote
        => Increment((Disposition.AddNew, quote.Date, (double)quote.Close));

    public new EmaResult Add((DateTime Date, double Value) price)  // intentionally hides provider Add
        => Increment((Disposition.AddNew, price.Date, price.Value));

    internal EmaResult Increment((Disposition disposition, DateTime date, double value) value)
    {
        // initialize
        EmaResult r = new(value.date)
        {
            Ema = Increment(value.date, value.value)
        };

        // propogate to observers
        if (HandleInboundResult(value.disposition, r)
            == Disposition.DoNothing)
        {
            return r;
        }

        // remove NaN
        r.Ema = r.Ema.NaN2Null();

        // save result
        switch (value.disposition)
        {
            case Disposition.AddNew:

                ProtectedResults.Add(r);
                break;
            case Disposition.AddOld:

                ProtectedResults.Add(r);
                throw new NotImplementedException();
            case Disposition.UpdateLast:
                throw new NotImplementedException();
            case Disposition.UpdateOld:
                throw new NotImplementedException();
            case Disposition.Delete:
                throw new NotImplementedException();
            case Disposition.DoNothing:
                break;
            default:
                break;
        }

        return r;
    }

    private double Increment(DateTime date, double newPrice)
    {

        double ema = double.NaN;
        int i = ProtectedResults
            .FindIndex(x => x.Date == date);

        // initialization periods
        if (i <= LookbackPeriods - 1)
        {
            // TODO: ignore duplicates
            SumValue += newPrice;

            // set first value
            if (i == LookbackPeriods - 1)
            {
                ema = SumValue / LookbackPeriods;
                SumValue = double.NaN;
            }
            return ema;
        }

        // normal
        return Increment(K, ProtectedTuples[i].Value, newPrice);
    }
}
