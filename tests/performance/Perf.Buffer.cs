namespace Performance;

// BUFFER-STYLE INDICATORS

[ShortRunJob]
public class BufferIndicators
{
    private static readonly IReadOnlyList<Quote> quotes
       = Data.GetDefault();

    private const int n = 14;

    [Benchmark]
    public AdlList AdlBuffer()
        => new() { quotes };

    [Benchmark]
    public AdxList AdxBuffer()
        => new(n) { quotes };

    [Benchmark]
    public AlmaList AlmaBuffer()
        => new(10, 0.85, 6) { quotes };

    [Benchmark]
    public AtrList AtrBuffer()
        => new(n) { quotes };

    [Benchmark]
    public BollingerBandsList BollingerBandsBuffer()
        => new(20, 2) { quotes };

    [Benchmark]
    public DemaList DemaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public EmaList EmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public EpmaList EpmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public HmaList HmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public KamaList KamaBuffer()
        => new(10, 2, 30) { quotes };

    [Benchmark]
    public MacdList MacdBuffer()
        => new(12, 26, 9) { quotes };

    [Benchmark]
    public MamaList MamaBuffer()
        => new(0.5, 0.05) { quotes };

    [Benchmark]
    public ObvList ObvBuffer()
        => new() { quotes };

    [Benchmark]
    public RsiList RsiBuffer()
        => new(n) { quotes };

    [Benchmark]
    public SmaList SmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public SmmaList SmmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public StochList StochBuffer()
        => new(14, 3, 3) { quotes };

    [Benchmark]
    public TemaList TemaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public TrList TrBuffer()
        => new() { quotes };

    [Benchmark]
    public VwmaList VwmaBuffer()
        => new(n) { quotes };

    [Benchmark]
    public WmaList WmaBuffer()
        => new(n) { quotes };
}
