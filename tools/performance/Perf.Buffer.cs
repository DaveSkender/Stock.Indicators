namespace Performance;

// BUFFER-STYLE INDICATORS

[ShortRunJob]
public class BufferIndicators
{
    private static readonly IReadOnlyList<Quote> q = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> o = Data.GetCompare();
    private const int n = 14;

    /* Parameter arguments should match the Perf.Series.cs equivalents */

    [Benchmark] public AdlList AdlList() => new() { q };
    [Benchmark] public AdxList AdxList() => new(n) { q };
    [Benchmark] public AlligatorList AlligatorList() => new() { q };
    [Benchmark] public AlmaList AlmaList() => new(9, 0.85, 6) { q };
    [Benchmark] public AroonList AroonList() => new() { q };
    [Benchmark] public AtrList AtrList() => new(n) { q };
    [Benchmark] public AtrStopList AtrStopList() => new() { q };
    [Benchmark] public AwesomeList AwesomeList() => new() { q };
    [Benchmark] public BetaList BetaList() => new(20, BetaType.Standard, q, o);
    [Benchmark] public BollingerBandsList BollingerBandsList() => new(20, 2) { q };
    [Benchmark] public BopList BopList() => new(n) { q };
    [Benchmark] public CciList CciList() => new(20) { q };
    [Benchmark] public ChaikinOscList ChaikinOscList() => new() { q };
    [Benchmark] public ChandelierList ChandelierList() => new() { q };
    [Benchmark] public ChopList ChopList() => new(n) { q };
    [Benchmark] public CmfList CmfList() => new() { q };
    [Benchmark] public CmoList CmoList() => new(n) { q };
    [Benchmark] public ConnorsRsiList ConnorsRsiList() => new() { q };
    [Benchmark] public CorrelationList CorrelationList() => new(20, q, o);
    [Benchmark] public DemaList DemaList() => new(n) { q };
    [Benchmark] public DojiList DojiList() => new() { q };
    [Benchmark] public DonchianList DonchianList() => new(20) { q };
    [Benchmark] public DpoList DpoList() => new(n) { q };
    [Benchmark] public DynamicList DynamicList() => new(20) { q };
    [Benchmark] public ElderRayList ElderRayList() => new() { q };
    [Benchmark] public EmaList EmaList() => new(20) { q };
    [Benchmark] public EpmaList EpmaList() => new(n) { q };
    [Benchmark] public FcbList FcbList() => new(n) { q };
    [Benchmark] public FisherTransformList FisherTransformList() => new(10) { q };
    [Benchmark] public ForceIndexList ForceIndexList() => new(13) { q };
    [Benchmark] public FractalList FractalList() => new() { q };
    [Benchmark] public GatorList GatorList() => new() { q };
    [Benchmark] public HeikinAshiList HeikinAshiList() => new() { q };
    [Benchmark] public HmaList HmaList() => new(n) { q };
    [Benchmark] public HurstList HurstList() => new(100) { q };
    [Benchmark] public KamaList KamaList() => new(10, 2, 30) { q };
    [Benchmark] public KeltnerList KeltnerList() => new() { q };
    [Benchmark] public KvoList KvoList() => new() { q };
    [Benchmark] public MacdList MacdList() => new(12, 26, 9) { q };
    [Benchmark] public MaEnvelopesList MaEnvelopesList() => new(20, 2.5, MaType.SMA) { q };
    [Benchmark] public MamaList MamaList() => new(0.5, 0.05) { q };
    [Benchmark] public MarubozuList MarubozuList() => new() { q };
    [Benchmark] public MfiList MfiList() => new() { q };
    [Benchmark] public ObvList ObvList() => new() { q };
    [Benchmark] public ParabolicSarList ParabolicSarList() => new() { q };
    [Benchmark] public PivotsList PivotsList() => new(2, 2, 20) { q };
    [Benchmark] public PmoList PmoList() => new() { q };
    [Benchmark] public QuotePartList QuotePartList() => new(CandlePart.Close) { q };
    [Benchmark] public PrsList PrsList() => new(int.MinValue, q, o);
    [Benchmark] public PvoList PvoList() => new() { q };
    [Benchmark] public RenkoList RenkoList() => new(2.5m) { q };
    [Benchmark] public RocList RocList() => new(20) { q };
    [Benchmark] public RollingPivotsList RollingPivotsList() => new(20, 0, PivotPointType.Standard) { q };
    [Benchmark] public RocWbList RocWbList() => new(20, 5, 5) { q };
    [Benchmark] public RsiList RsiList() => new(n) { q };
    [Benchmark] public SlopeList SlopeList() => new(n) { q };
    [Benchmark] public SmaList SmaList() => new(n) { q };
    [Benchmark] public SmaAnalysisList SmaAnalysisList() => new(10) { q };
    [Benchmark] public SmiList SmiList() => new(13, 25, 2, 3) { q };
    [Benchmark] public SmmaList SmmaList() => new(n) { q };
    [Benchmark] public StarcBandsList StarcBandsList() => new(5, 2, 10) { q };
    [Benchmark] public StcList StcList() => new(10, 23, 50) { q };
    [Benchmark] public StdDevList StdDevList() => new(20) { q };
    [Benchmark] public StdDevChannelsList StdDevChannelsList() => new(20, 2) { q };
    [Benchmark] public StochList StochList() => new(14, 3, 3) { q };
    [Benchmark] public StochRsiList StochRsiList() => new(14, 14, 3, 1) { q };
    [Benchmark] public SuperTrendList SuperTrendList() => new() { q };
    [Benchmark] public T3List T3List() => new(5, 0.7) { q };
    [Benchmark] public TemaList TemaList() => new(20) { q };
    [Benchmark] public TrList TrList() => new() { q };
    [Benchmark] public TrixList TrixList() => new(n) { q };
    [Benchmark] public TsiList TsiList() => new(25, 13, 7) { q };
    [Benchmark] public UlcerIndexList UlcerIndexList() => new(n) { q };
    [Benchmark] public UltimateList UltimateList() => new(7, 14, 28) { q };
    [Benchmark] public VolatilityStopList VolatilityStopList() => new(7, 3) { q };
    [Benchmark] public VortexList VortexList() => new(n) { q };
    [Benchmark] public VwapList VwapList() => new(q[0].Timestamp) { q };
    [Benchmark] public VwmaList VwmaList() => new(n) { q };
    [Benchmark] public WilliamsRList WilliamsRList() => new() { q };
    [Benchmark] public WmaList WmaList() => new(n) { q };
    [Benchmark] public ZigZagList ZigZagList() => new() { q };
}
