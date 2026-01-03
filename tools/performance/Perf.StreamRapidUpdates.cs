namespace Performance;

// STREAM-STYLE INDICATORS - RAPID SAME-CANDLE UPDATES
// Tests the performance benefit of StreamHubState for rapid updates to the same candle

[ShortRunJob]
public class StreamRapidUpdates
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private const int lookbackPeriods = 14;
    private const int rapidUpdates = 100; // Number of rapid updates to simulate

    // PMO parameters
    private const int smoothing = 35;
    private const int dblSmoothing = 20;

    // TSI parameters
    private const int smoothPeriods = 13;

    // Signal periods (shared by PMO and TSI)
    private const int signalPeriods = 7;

    // ConnorsRSI parameters
    private const int rsiPeriods = 3;
    private const int streakPeriods = 2;
    private const int rankPeriods = 100;

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

    // PMO - Hub rapid updates
    [Benchmark]
    public IReadOnlyList<PmoResult> Pmo_Hub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        PmoHub observer = quoteHub.ToPmoHub(smoothing, dblSmoothing, signalPeriods);

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

        IReadOnlyList<PmoResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // PMO - HubState rapid updates
    [Benchmark]
    public IReadOnlyList<PmoResult> Pmo_HubState_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        PmoHubState observer = quoteHub.ToPmoHubState(smoothing, dblSmoothing, signalPeriods);

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

        IReadOnlyList<PmoResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // TSI - Hub rapid updates
    [Benchmark]
    public IReadOnlyList<TsiResult> Tsi_Hub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        TsiHub observer = quoteHub.ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods);

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

        IReadOnlyList<TsiResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // TSI - HubState rapid updates
    [Benchmark]
    public IReadOnlyList<TsiResult> Tsi_HubState_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        TsiHubState observer = quoteHub.ToTsiHubState(lookbackPeriods, smoothPeriods, signalPeriods);

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

        IReadOnlyList<TsiResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // ConnorsRSI - Hub rapid updates
    [Benchmark]
    public IReadOnlyList<ConnorsRsiResult> ConnorsRsi_Hub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        ConnorsRsiHub observer = quoteHub.ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);

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

        IReadOnlyList<ConnorsRsiResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // ConnorsRSI - HubState rapid updates
    [Benchmark]
    public IReadOnlyList<ConnorsRsiResult> ConnorsRsi_HubState_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        ConnorsRsiHubState observer = quoteHub.ToConnorsRsiHubState(rsiPeriods, streakPeriods, rankPeriods);

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

        IReadOnlyList<ConnorsRsiResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // StochRSI - Hub rapid updates
    [Benchmark]
    public IReadOnlyList<StochRsiResult> StochRsi_Hub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        StochRsiHub observer = quoteHub.ToStochRsiHub();

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

        IReadOnlyList<StochRsiResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // StochRSI - HubState rapid updates
    [Benchmark]
    public IReadOnlyList<StochRsiResult> StochRsi_HubState_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        StochRsiHubState observer = quoteHub.ToStochRsiHubState();

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

        IReadOnlyList<StochRsiResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // Ichimoku - Hub rapid updates
    [Benchmark]
    public IReadOnlyList<IchimokuResult> Ichimoku_Hub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        IchimokuHub observer = quoteHub.ToIchimokuHub();

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

        IReadOnlyList<IchimokuResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }

    // Ichimoku - HubState rapid updates
    [Benchmark]
    public IReadOnlyList<IchimokuResult> Ichimoku_HubState_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);

        IchimokuHubState observer = quoteHub.ToIchimokuHubState();

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

        IReadOnlyList<IchimokuResult> results = observer.Results;

        observer.Unsubscribe();
        quoteHub.EndTransmission();

        return results;
    }
}
