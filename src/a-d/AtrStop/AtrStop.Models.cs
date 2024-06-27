namespace Skender.Stock.Indicators;

public record struct AtrStopResult(
    DateTime Timestamp,
    decimal? AtrStop,
    decimal? BuyStop,
    decimal? SellStop
) : IResult;
