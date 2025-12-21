namespace Performance;
#pragma warning disable CS1591, CA1707

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
    private static readonly IReadOnlyList<QuoteD> quotesD = quotes.ToQuoteDList();
    private static readonly IReadOnlyList<QuoteX> quotesX = quotes.ToQuoteXList();

    private const int FastPeriods = 12;
    private const int SlowPeriods = 26;
    private const int SignalPeriods = 9;

    /// <summary>
    /// Baseline: MACD calculation with standard Quote (decimal-based).
    /// Uses the standard ToMacd() extension method.
    /// </summary>
    [Benchmark(Baseline = true)]
    public object Macd_Decimal()
        => quotes.ToMacd(FastPeriods, SlowPeriods, SignalPeriods);

    /// <summary>
    /// MACD calculation with QuoteD (double-based).
    /// Note: Standard implementation doesn't have a QuoteD-specific MACD,
    /// so this converts through IReusable interface.
    /// </summary>
    [Benchmark]
    public object Macd_Double()
    {
        return quotesD.ToMacdQuoteD(FastPeriods, SlowPeriods, SignalPeriods);
    }

    /// <summary>
    /// MACD calculation with QuoteX (long-based internal storage).
    /// Uses experimental ToMacdX() method that leverages internal long arithmetic.
    /// </summary>
    [Benchmark]
    public object Macd_Long()
        => quotesX.ToMacdX(FastPeriods, SlowPeriods, SignalPeriods);
}
