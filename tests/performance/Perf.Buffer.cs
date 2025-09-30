namespace Performance;

// BUFFER-STYLE INDICATORS

[ShortRunJob]
public class BufferIndicators
{
    private static readonly IReadOnlyList<Quote> quotes
       = Data.GetDefault();

    private const int n = 14;

    [Benchmark]
    public AdxList AdxBuffer()
        => new(n) { quotes };

    [Benchmark]
    public EmaList EmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public SmaList SmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public WmaList WmaBuffer()
        => new(n) { quotes };
}
