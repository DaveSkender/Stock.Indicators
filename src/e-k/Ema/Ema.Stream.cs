namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)
public class Ema
{
    // initialize streaming base
    internal Ema(IEnumerable<(DateTime, double)> tpQuotes, int lookbackPeriods)
    {
        List<(DateTime Date, double Value)>? tpList = tpQuotes.ToTupleList();

        List<EmaResult>? baseline = tpList.CalcEma(lookbackPeriods).ToList();

        // store results
        ProtectedResults = baseline;
        K = 2d / (lookbackPeriods + 1);

        EmaResult? last = baseline.LastOrDefault();

        if (last != null)
        {
            LastDate = last.Date;
            LastEma = (last.Ema == null) ? double.NaN : (double)last.Ema;
        }
    }

    internal double K { get; set; }
    internal DateTime LastDate { get; set; }
    internal double LastEma { get; set; }
    internal List<EmaResult> ProtectedResults { get; set; }

    public IEnumerable<EmaResult> Results => ProtectedResults;
    public IEnumerable<EmaResult> Add(
        Quote quote,
        CandlePart candlePart = CandlePart.Close)
    {
        if (quote == null)
        {
            throw new InvalidQuotesException(nameof(quote), quote, "No quote provided.");
        }

        (DateTime Date, double Value) tuple = quote.ToBasicTuple(candlePart);
        return Add(tuple);
    }

    public IEnumerable<EmaResult> Add(
        (DateTime Date, double Value) tuple)
    {
        // calculate incremental value
        double ema = Increment(tuple.Value, LastEma, K);

        // update bar
        if (tuple.Date == LastDate)
        {
            EmaResult? e = ProtectedResults.Find(tuple.Date);
            if (e != null)
            {
                e.Ema = ema;
            }
        }

        // add new bar
        else if (tuple.Date > LastDate)
        {
            EmaResult r = new() { Date = tuple.Date, Ema = ema };

            LastDate = tuple.Date;
            LastEma = ema;

            ProtectedResults.Add(r);
        }

        return Results;
    }

    internal static double Increment(double newValue, double lastEma, double k)
        => lastEma + (k * (newValue - lastEma));
}