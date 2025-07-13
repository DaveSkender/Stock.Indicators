namespace Skender.Stock.Indicators;

public static partial class SuperTrend
{
    // SuperTrend Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("SuperTrend")
            .WithId("SUPERTREND")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SuperTrend calculation", isRequired: false, defaultValue: 10, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", description: "Multiplier for the ATR calculation", isRequired: false, defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddResult("SuperTrend", "SuperTrend", ResultType.Default, isDefault: true)
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: false)
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false)
            .Build();
}