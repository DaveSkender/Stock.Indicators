namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class ZigZagResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public decimal? ZigZag { get; set; } // zig zag line
    public string? PointType { get; set; } // indicates a specific point and type e.g. H or L
    public decimal? RetraceHigh { get; set; } // zig zag retrace high line
    public decimal? RetraceLow { get; set; } // zig zag retrace low line

    double IReusableResult.Value => ZigZag.Null2NaN();
}

internal class ZigZagEval
{
    internal int Index { get; set; }
    internal decimal? High { get; set; }
    internal decimal? Low { get; set; }
}

internal class ZigZagPoint
{
    internal int Index { get; set; }
    internal decimal? Value { get; set; }
    internal string? PointType { get; set; }
}
