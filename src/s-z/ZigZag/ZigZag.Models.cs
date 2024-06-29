namespace Skender.Stock.Indicators;

public readonly record struct ZigZagResult
(
    DateTime Timestamp,
    decimal? ZigZag,      // zig zag line
    string? PointType,    // indicates a specific point and type e.g. H or L
    decimal? RetraceHigh, // zig zag retrace high line
    decimal? RetraceLow   // zig zag retrace low line
) : IReusable
{
    double IReusable.Value => ZigZag.Null2NaN();
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
