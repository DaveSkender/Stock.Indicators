namespace Skender.Stock.Indicators;

public record struct RollingPivotsResult : IResult, IPivotPoint
{
    public DateTime Timestamp { get; set; }
    public decimal? R4 { get; set; }
    public decimal? R3 { get; set; }
    public decimal? R2 { get; set; }
    public decimal? R1 { get; set; }
    public decimal? PP { get; set; }
    public decimal? S1 { get; set; }
    public decimal? S2 { get; set; }
    public decimal? S3 { get; set; }
    public decimal? S4 { get; set; }
}
