namespace Skender.Stock.Indicators;

public readonly record struct AtrStopResult(
    DateTime Timestamp,
    decimal? AtrStop,
    decimal? BuyStop,
    decimal? SellStop
) : IResult;
