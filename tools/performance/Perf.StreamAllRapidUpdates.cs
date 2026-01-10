namespace Performance;

// COMPREHENSIVE STREAM HUB RAPID UPDATE BENCHMARKS
// Benchmarks for all StreamHub indicators testing rapid same-candle updates
// This simulates real-time streaming scenarios where the last candle receives frequent price updates

[ShortRunJob]
public class StreamAllRapidUpdates
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private const int n = 14;
    private const int rapidUpdates = 100; // Number of rapid updates to simulate

    /* NOTE: These benchmarks test the performance of StreamHub indicators
     * under rapid same-candle update scenarios. This is relevant for live
     * trading data where the most recent candle receives frequent updates.
     *
     * For indicators with complex inter-candle state (RSI, EMA, PMO, etc.),
     * the StreamHubState pattern with state caching can provide significant
     * performance improvements. For simple window-based indicators (SMA, etc.),
     * the overhead may exceed the benefit.
     *
     * See Perf.StreamRapidUpdates.cs for detailed benchmarks comparing
     * Hub vs HubState implementations for select indicators. */

    // Helper method to create updated quote
    private static Quote CreateUpdatedQuote(Quote lastQuote, int iteration) => new()
    {
        Timestamp = lastQuote.Timestamp,
        Open = lastQuote.Open,
        High = lastQuote.High + (iteration * 0.01m),
        Low = lastQuote.Low,
        Close = lastQuote.Close + (iteration * 0.01m),
        Volume = lastQuote.Volume
    };

    // ===== A =====

    [Benchmark]
    public object AdlHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToAdlHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object AdxHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToAdxHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object AlligatorHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToAlligatorHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object AlmaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToAlmaHub(9, 0.85, 6);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object AroonHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToAroonHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object AtrHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToAtrHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object AtrStopHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToAtrStopHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object AwesomeHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToAwesomeHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== B =====

    [Benchmark]
    public object BollingerBandsHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToBollingerBandsHub(20, 2);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object BopHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToBopHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== C =====

    [Benchmark]
    public object CciHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToCciHub(20);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object ChaikinOscHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToChaikinOscHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object ChandelierHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToChandelierHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object ChopHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToChopHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object CmfHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToCmfHub(20);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object CmoHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToCmoHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // Note: ConnorsRsi already has detailed benchmarks in Perf.StreamRapidUpdates.cs

    // ===== D =====

    [Benchmark]
    public object DemaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToDemaHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object DojiHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToDojiHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object DonchianHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToDonchianHub(20);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object DpoHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToDpoHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object DynamicHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToDynamicHub(n, 0.6);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== E =====

    [Benchmark]
    public object ElderRayHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToElderRayHub(13);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object EmaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToEmaHub(20);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object EpmaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToEpmaHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== F =====

    [Benchmark]
    public object FcbHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToFcbHub(2);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object FisherTransformHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToFisherTransformHub(10);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object ForceIndexHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToForceIndexHub(2);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object FractalHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToFractalHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== G =====

    [Benchmark]
    public object GatorHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToGatorHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== H =====

    [Benchmark]
    public object HeikinAshiHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToHeikinAshiHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object HmaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToHmaHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object HtTrendlineHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToHtTrendlineHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object HurstHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToHurstHub(100);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== I =====

    [Benchmark]
    public object IchimokuHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToIchimokuHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== K =====

    [Benchmark]
    public object KamaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToKamaHub(10, 2, 30);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object KeltnerHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToKeltnerHub(20, 2, 10);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object KvoHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToKvoHub(34, 55, 13);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== M =====

    [Benchmark]
    public object MacdHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToMacdHub(12, 26, 9);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object MaEnvelopesHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToMaEnvelopesHub(20, 2.5, MaType.SMA);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object MamaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToMamaHub(0.5, 0.05);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object MarubozuHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToMarubozuHub(95);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object MfiHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToMfiHub(14);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== O =====

    [Benchmark]
    public object ObvHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToObvHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== P =====

    [Benchmark]
    public object ParabolicSarHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToParabolicSarHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object PivotPointsHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToPivotPointsHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object PivotsHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToPivotsHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // Note: PMO already has detailed benchmarks in Perf.StreamRapidUpdates.cs

    [Benchmark]
    public object PvoHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToPvoHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== R =====

    [Benchmark]
    public object RenkoHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToRenkoHub(2.5m);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object RocHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToRocHub(20);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object RocWbHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToRocWbHub(20, 5, 5);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object RollingPivotsHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToRollingPivotsHub(20, 0, PivotPointType.Standard);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // Note: RSI already has detailed benchmarks in Perf.StreamRapidUpdates.cs

    // ===== S =====

    [Benchmark]
    public object SlopeHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToSlopeHub(20);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // Note: SMA already has detailed benchmarks in Perf.StreamRapidUpdates.cs

    [Benchmark]
    public object SmaAnalysisHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToSmaAnalysisHub(20);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object SmiHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToSmiHub(13, 25, 2, 3);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object SmmaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToSmmaHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object StarcBandsHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToStarcBandsHub(5, 2, 10);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object StcHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToStcHub(10, 23, 50);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // Note: StdDev already has detailed benchmarks in Perf.StreamRapidUpdates.cs

    [Benchmark]
    public object StochHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToStochHub(n, 3, 3);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object StochRsiHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToStochRsiHub(n, n, 3, 1);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object SuperTrendHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToSuperTrendHub(10, 3);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== T =====

    [Benchmark]
    public object T3Hub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToT3Hub(5, 0.7);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object TemaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToTemaHub(20);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object TrHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToTrHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object TrixHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToTrixHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // Note: TSI already has detailed benchmarks in Perf.StreamRapidUpdates.cs

    // ===== U =====

    [Benchmark]
    public object UlcerIndexHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToUlcerIndexHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object UltimateHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToUltimateHub(7, n, 28);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== V =====

    [Benchmark]
    public object VolatilityStopHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToVolatilityStopHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object VortexHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToVortexHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object VwapHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToVwapHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object VwmaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToVwmaHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    // ===== W =====

    [Benchmark]
    public object WilliamsRHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToWilliamsRHub();
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }

    [Benchmark]
    public object WmaHub_RapidUpdates()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        var observer = quoteHub.ToWmaHub(n);
        Quote lastQuote = quotes[^1];
        for (int i = 0; i < rapidUpdates; i++)
        {
            quoteHub.Add(CreateUpdatedQuote(lastQuote, i));
        }
        var results = observer.Results;
        observer.Unsubscribe();
        quoteHub.EndTransmission();
        return results;
    }
}
