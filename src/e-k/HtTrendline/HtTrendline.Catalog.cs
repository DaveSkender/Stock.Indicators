namespace Skender.Stock.Indicators;

public static partial class HtTrendline
{
    // HTL Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Hilbert Transform Instantaneous Trendline") // From catalog.bak.json
            .WithId("HTL") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage) // From catalog.bak.json Category: "MovingAverage"
            // No parameters for HTL in catalog.bak.json
            .AddResult("DcPeriods", "Dominant Cycle Periods", ResultType.Default, isDefault: false) // From HtlResult model
            .AddResult("Trendline", "Trendline", ResultType.Default, isDefault: true) // From HtlResult model
            .AddResult("SmoothPrice", "Smooth Price", ResultType.Default, isDefault: false) // From HtlResult model
            .Build();

    // No StreamListing for HTL.
    // No BufferListing for HTL.
}
