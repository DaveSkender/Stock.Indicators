namespace Skender.Stock.Indicators;

internal interface IPivotPoint
{
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

[Serializable]
public sealed class PivotPointsResult : ResultBase, IPivotPoint
{
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

public enum PivotPointType
{
    // do not modify numbers,
    // just add new random numbers if extending

    Standard = 0,
    Camarilla = 1,
    Demark = 2,
    Fibonacci = 3,
    Woodie = 4
}
