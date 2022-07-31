namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)
internal class EmaBase
{
    // initialize base
    internal EmaBase(IEnumerable<(DateTime, double)> tpQuotes, int lookbackPeriods)
    {
        K = 2d / (lookbackPeriods + 1);

        ProtectedResults = tpQuotes
            .ToSortedList()
            .CalcEma(lookbackPeriods);
    }

    // properties
    internal double K { get; set; }
    internal List<EmaResult> ProtectedResults { get; set; }

    public IEnumerable<EmaResult> Results => ProtectedResults;

    // methods
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

            EmaResult r = new(tuple.Date) { Ema = newEma };
            ProtectedResults.Add(r);
        }

        return Results;
    }

    // incremental calculation
    internal static double Increment(double newValue, double lastEma, double k)
        => lastEma + (k * (newValue - lastEma));

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
}
