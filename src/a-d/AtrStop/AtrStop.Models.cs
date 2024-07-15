namespace Skender.Stock.Indicators;

public record AtrStopResult(
    DateTime Timestamp,
    decimal? AtrStop,
    decimal? BuyStop,
    decimal? SellStop
) : ISeries;
