namespace Performance;

// STREAM-STYLE INDICATORS - RAPID SAME-CANDLE UPDATES
// Tests the performance benefit of StreamHubState for rapid updates to the same candle

[ShortRunJob]
public class StreamRapidUpdates
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private const int lookbackPeriods = 14;
    private const int rapidUpdates = 100; // Number of rapid updates to simulate

    /* SETUP/CLEANUP
     *
     * Each benchmark will prepopulate the hub with historical data,
     * then perform multiple rapid updates to the latest candle.
     * This simulates real-time streaming scenarios where the last
     * candle receives frequent price updates. */

    // RSI HUB - ORIGINAL
    [Benchmark]
    public object RsiHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        RsiHub observer = quoteHub.ToRsiHub(lookbackPeriods);

        // Simulate rapid updates to the last candle
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            // Create slightly modified version of the last quote
            Quote updatedQuote = new() {
                Timestamp = lastQuote.Timestamp,
                Open = lastQuote.Open,
                High = lastQuote.High + (i * 0.01m),
                Low = lastQuote.Low,
                Close = lastQuote.Close + (i * 0.01m),
                Volume = lastQuote.Volume
            };

            quoteHub.Add(updatedQuote);
        }

        IReadOnlyList<RsiResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // RSI HUB STATE - OPTIMIZED
    [Benchmark]
    public object RsiHubState_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        RsiHubState observer = quoteHub.ToRsiHubState(lookbackPeriods);

        // Simulate rapid updates to the last candle
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            // Create slightly modified version of the last quote
            Quote updatedQuote = new() {
                Timestamp = lastQuote.Timestamp,
                Open = lastQuote.Open,
                High = lastQuote.High + (i * 0.01m),
                Low = lastQuote.Low,
                Close = lastQuote.Close + (i * 0.01m),
                Volume = lastQuote.Volume
            };

            quoteHub.Add(updatedQuote);
        }

        IReadOnlyList<RsiResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // SMA HUB - ORIGINAL
    [Benchmark]
    public object SmaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        SmaHub observer = quoteHub.ToSmaHub(lookbackPeriods);

        // Simulate rapid updates to the last candle
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            Quote updatedQuote = new() {
                Timestamp = lastQuote.Timestamp,
                Open = lastQuote.Open,
                High = lastQuote.High + (i * 0.01m),
                Low = lastQuote.Low,
                Close = lastQuote.Close + (i * 0.01m),
                Volume = lastQuote.Volume
            };

            quoteHub.Add(updatedQuote);
        }

        IReadOnlyList<SmaResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // SMA HUB STATE - OPTIMIZED
    [Benchmark]
    public object SmaHubState_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        SmaHubState observer = quoteHub.ToSmaHubState(lookbackPeriods);

        // Simulate rapid updates to the last candle
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            Quote updatedQuote = new() {
                Timestamp = lastQuote.Timestamp,
                Open = lastQuote.Open,
                High = lastQuote.High + (i * 0.01m),
                Low = lastQuote.Low,
                Close = lastQuote.Close + (i * 0.01m),
                Volume = lastQuote.Volume
            };

            quoteHub.Add(updatedQuote);
        }

        IReadOnlyList<SmaResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // STDDEV HUB - ORIGINAL
    [Benchmark]
    public object StdDevHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        StdDevHub observer = quoteHub.ToStdDevHub(lookbackPeriods);

        // Simulate rapid updates to the last candle
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            Quote updatedQuote = new() {
                Timestamp = lastQuote.Timestamp,
                Open = lastQuote.Open,
                High = lastQuote.High + (i * 0.01m),
                Low = lastQuote.Low,
                Close = lastQuote.Close + (i * 0.01m),
                Volume = lastQuote.Volume
            };

            quoteHub.Add(updatedQuote);
        }

        IReadOnlyList<StdDevResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // STDDEV HUB STATE - OPTIMIZED
    [Benchmark]
    public object StdDevHubState_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        StdDevHubState observer = quoteHub.ToStdDevHubState(lookbackPeriods);

        // Simulate rapid updates to the last candle
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            Quote updatedQuote = new() {
                Timestamp = lastQuote.Timestamp,
                Open = lastQuote.Open,
                High = lastQuote.High + (i * 0.01m),
                Low = lastQuote.Low,
                Close = lastQuote.Close + (i * 0.01m),
                Volume = lastQuote.Volume
            };

            quoteHub.Add(updatedQuote);
        }

        IReadOnlyList<StdDevResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }
}
