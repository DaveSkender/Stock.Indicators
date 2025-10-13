namespace Performance;

// BUFFER-STYLE INDICATORS

[ShortRunJob]
public class BufferIndicators
{
    private static readonly IReadOnlyList<Quote> quotes
       = Data.GetDefault();

    private const int n = 14;

    [Benchmark]
    public AdlList AdlList()
        => new() { quotes };

    [Benchmark]
    public AdxList AdxList()
        => new(n) { quotes };

    [Benchmark]
    public AlmaList AlmaList()
        => new(10, 0.85, 6) { quotes };

    [Benchmark]
    public AtrList AtrList()
        => new(n) { quotes };

    [Benchmark]
    public BollingerBandsList BollingerBandsList()
        => new(20, 2) { quotes };

    [Benchmark]
    public DemaList DemaList()
        => new(n) { quotes };

    [Benchmark]
    public EmaList EmaList()
        => new(n) { quotes };

    [Benchmark]
    public EpmaList EpmaList()
        => new(n) { quotes };

    [Benchmark]
    public HmaList HmaList()
        => new(n) { quotes };

    [Benchmark]
    public KamaList KamaList()
        => new(10, 2, 30) { quotes };

    [Benchmark]
    public MacdList MacdList()
        => new(12, 26, 9) { quotes };

    [Benchmark]
    public MamaList MamaList()
        => new(0.5, 0.05) { quotes };

    [Benchmark]
    public ObvList ObvList()
        => new() { quotes };

    [Benchmark]
    public RsiList RsiList()
        => new(n) { quotes };

    [Benchmark]
    public RocList RocList()
        => new(20) { quotes };

    [Benchmark]
    public SmaList SmaList()
        => new(n) { quotes };

    [Benchmark]
    public SmmaList SmmaList()
        => new(n) { quotes };

    [Benchmark]
    public SmiList SmiList()
        => new(13, 25, 2, 3) { quotes };

    [Benchmark]
    public StarcBandsList StarcBandsList()
        => new(5, 2, 10) { quotes };

    [Benchmark]
    public StochList StochList()
        => new(14, 3, 3) { quotes };

    [Benchmark]
    public StochRsiList StochRsiList()
        => new(14, 14, 3, 1) { quotes };

    [Benchmark]
    public T3List T3List()
        => new(5, 0.7) { quotes };

    [Benchmark]
    public TemaList TemaList()
        => new(n) { quotes };

    [Benchmark]
    public TrList TrList()
        => new() { quotes };

    [Benchmark]
    public UltimateList UltimateList()
        => new(7, 14, 28) { quotes };

    [Benchmark]
    public VolatilityStopList VolatilityStopList()
        => new(7, 3) { quotes };

    [Benchmark]
    public VortexList VortexList()
        => new(n) { quotes };

    [Benchmark]
    public VwapList VwapList()
        => new(quotes[0].Timestamp) { quotes };

    [Benchmark]
    public VwmaList VwmaList()
        => new(n) { quotes };

    [Benchmark]
    public WmaList WmaList()
        => new(n) { quotes };
}
