namespace Performance;

/// <summary>
/// Benchmarks for comparing memory size and allocation patterns
/// of different Quote implementations: Quote, QuoteD, and QuoteX.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class QuoteSizeComparison
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();

    /// <summary>
    /// Baseline: Create list of standard Quote (decimal-based).
    /// </summary>
    [Benchmark(Baseline = true)]
    public object CreateQuoteList()
    {
        List<Quote> list = new(quotes.Count);
        foreach (Quote q in quotes)
        {
            list.Add(new Quote(
                Timestamp: q.Timestamp,
                Open: q.Open,
                High: q.High,
                Low: q.Low,
                Close: q.Close,
                Volume: q.Volume));
        }

        return list;
    }

    /// <summary>
    /// Create list of QuoteD (double-based) for comparison.
    /// </summary>
    [Benchmark]
    public object CreateQuoteDList()
        => quotes.ToQuoteDList();

    /// <summary>
    /// Create list of QuoteX (long-based internal storage) for comparison.
    /// </summary>
    [Benchmark]
    public object CreateQuoteXList()
        => quotes.ToQuoteXList();

    /// <summary>
    /// Report individual object sizes for documentation purposes.
    /// This doesn't run as a benchmark but provides size information.
    /// </summary>
    public static void ReportSizes()
    {
        // Note: These are approximate sizes in memory
        // Actual runtime size may vary due to object header, padding, etc.

        // Quote: DateTime (8) + 5 decimal fields (5 * 16 = 80) = 88 bytes base
        // QuoteD: DateTime (8) + 5 double fields (5 * 8 = 40) = 48 bytes base  
        // QuoteX: long (8) + DateTimeKind (4) + 5 long fields (5 * 8 = 40) = 52 bytes base

        Console.WriteLine("Approximate base sizes (excluding object overhead):");
        Console.WriteLine("  Quote (decimal):  ~88 bytes");
        Console.WriteLine("  QuoteD (double):  ~48 bytes");
        Console.WriteLine("  QuoteX (long):    ~52 bytes");
        Console.WriteLine();
        Console.WriteLine("For 502 quotes (typical test data):");
        Console.WriteLine($"  Quote list:   ~{88 * 502 / 1024.0:F2} KB");
        Console.WriteLine($"  QuoteD list:  ~{48 * 502 / 1024.0:F2} KB");
        Console.WriteLine($"  QuoteX list:  ~{52 * 502 / 1024.0:F2} KB");
    }
}
