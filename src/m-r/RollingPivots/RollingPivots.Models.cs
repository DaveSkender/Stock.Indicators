namespace Skender.Stock.Indicators;

public readonly record struct RollingPivotsResult : IResult, IPivotPoint
{
    public DateTime Timestamp { get; init; }

    public decimal? PP { get; init; }

    public decimal? S1 { get; init; }
    public decimal? S2 { get; init; }
    public decimal? S3 { get; init; }
    public decimal? S4 { get; init; }

    public decimal? R1 { get; init; }
    public decimal? R2 { get; init; }
    public decimal? R3 { get; init; }
    public decimal? R4 { get; init; }
}
