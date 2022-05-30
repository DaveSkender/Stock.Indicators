namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)
public class Ema
{
    // initialize streaming base
    internal Ema(IEnumerable<(DateTime, double)> tpQuotes, int lookbackPeriods)
    {
        List<(DateTime Date, double Value)>? tpList = tpQuotes.ToSortedList();

        List<EmaResult>? baseline = tpList.CalcEma(lookbackPeriods).ToList();

        // store results
        ProtectedResults = baseline;
        K = 2d / (lookbackPeriods + 1);
    }

    internal double K { get; set; }
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
        // get last value
        int lastIndex = ProtectedResults.Count - 1;
        EmaResult last = ProtectedResults[lastIndex];

        // update bar
        if (tuple.Date == last.Date)
        {
            // get prior last EMA
            EmaResult prior = ProtectedResults[lastIndex - 1];

            double priorEma = (prior.Ema == null) ? double.NaN : (double)prior.Ema;
            last.Ema = Increment(tuple.Value, priorEma, K);
        }

        // add new bar
        else if (tuple.Date > last.Date)
        {
            // calculate incremental value
            double lastEma = (last.Ema == null) ? double.NaN : (double)last.Ema;
            double newEma = Increment(tuple.Value, lastEma, K);

            EmaResult r = new() { Date = tuple.Date, Ema = newEma };
            ProtectedResults.Add(r);
        }

        return Results;
    }

    internal static double Increment(double newValue, double lastEma, double k)
        => lastEma + (k * (newValue - lastEma));
}