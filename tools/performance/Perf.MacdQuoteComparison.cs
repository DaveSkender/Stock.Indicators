namespace Performance;

/// <summary>
/// Benchmarks for comparing MACD calculation performance
/// across different Quote implementations: Quote, QuoteD, and QuoteX.
/// All use identical parameters (12, 26, 9) for fair comparison.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class MacdQuoteComparison
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly List<QuoteD> quotesD = quotes.ToQuoteDList();
    private static readonly List<QuoteX> quotesX = quotes.ToQuoteXList();

    private const int FastPeriods = 12;
    private const int SlowPeriods = 26;
    private const int SignalPeriods = 9;

    /// <summary>
    /// Baseline: MACD calculation with standard Quote (decimal-based).
    /// Uses the standard ToMacd() extension method.
    /// </summary>
    [Benchmark(Baseline = true)]
    public object MacdWithQuote()
        => quotes.ToMacd(FastPeriods, SlowPeriods, SignalPeriods);

    /// <summary>
    /// MACD calculation with QuoteD (double-based).
    /// Note: Standard implementation doesn't have a QuoteD-specific MACD,
    /// so this converts through IReusable interface.
    /// </summary>
    [Benchmark]
    public object MacdWithQuoteD()
    {
        // QuoteD doesn't implement IQuote, only IReusable
        // So we can't directly use it with ToMacd(IQuote)
        // This measures the conversion overhead
        IReadOnlyList<Quote> converted = quotesD
            .Select(qd => new Quote(
                qd.Timestamp,
                (decimal)qd.Open,
                (decimal)qd.High,
                (decimal)qd.Low,
                (decimal)qd.Close,
                (decimal)qd.Volume))
            .ToList();

        return converted.ToMacd(FastPeriods, SlowPeriods, SignalPeriods);
    }

    /// <summary>
    /// MACD calculation with QuoteX (long-based internal storage).
    /// Uses experimental ToMacdX() method that leverages internal long arithmetic.
    /// </summary>
    [Benchmark]
    public object MacdWithQuoteX()
        => quotesX.ToMacdX(FastPeriods, SlowPeriods, SignalPeriods);
}
