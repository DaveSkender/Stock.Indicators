namespace Skender.Stock.Indicators;

internal interface IPivotPoint
{
    decimal? R4 { get; }
    decimal? R3 { get; }
    decimal? R2 { get; }
    decimal? R1 { get; }
    decimal? Pp { get; }
    decimal? S1 { get; }
    decimal? S2 { get; }
    decimal? S3 { get; }
    decimal? S4 { get; }
}

public readonly record struct PivotPointsResult : IResult, IPivotPoint
{
    public DateTime Timestamp { get; init; }

    public decimal? Pp { get; init; }

    public decimal? S1 { get; init; }
    public decimal? S2 { get; init; }
    public decimal? S3 { get; init; }
    public decimal? S4 { get; init; }

    public decimal? R1 { get; init; }
    public decimal? R2 { get; init; }
    public decimal? R3 { get; init; }
    public decimal? R4 { get; init; }
}

internal record WindowPoint : IPivotPoint
{
    public decimal? Pp { get; init; }

    public decimal? S1 { get; init; }
    public decimal? S2 { get; init; }
    public decimal? S3 { get; init; }
    public decimal? S4 { get; init; }

    public decimal? R1 { get; init; }
    public decimal? R2 { get; init; }
    public decimal? R3 { get; init; }
    public decimal? R4 { get; init; }
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
