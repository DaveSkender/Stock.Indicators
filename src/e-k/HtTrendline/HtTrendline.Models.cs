namespace Skender.Stock.Indicators;

public record HtlResult
(
    DateTime Timestamp,
    int? DcPeriods,
    double? Trendline,
    double? SmoothPrice
) : Reusable(Timestamp, Trendline.Null2NaN());
