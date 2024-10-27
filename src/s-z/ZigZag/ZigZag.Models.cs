namespace Skender.Stock.Indicators;

[Serializable]
public record ZigZagResult
(
    DateTime Timestamp,
    decimal? ZigZag = null,      // zig zag line
    string? PointType = null,    // indicates a specific point and type e.g. H or L
    decimal? RetraceHigh = null, // zig zag retrace high line
    decimal? RetraceLow = null   // zig zag retrace low line
) : IReusable
{
    public double Value => ZigZag.Null2NaN();
}

internal class ZigZagEval
{
    internal int Index { get; init; }
    internal decimal? High { get; set; }
    internal decimal? Low { get; set; }
}

internal class ZigZagPoint
{
    internal int Index { get; set; }
    internal decimal? Value { get; set; }
    internal string? PointType { get; set; }
}
