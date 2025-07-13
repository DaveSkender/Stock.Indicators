namespace Skender.Stock.Indicators;

public static partial class VolatilityStop
{
    // Volatility Stop Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Volatility Stop")
            .WithId("VOL-STOP")
            .WithStyle(Style.Series)
            .WithCategory(Category.StopAndReverse)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the volatility calculation", isRequired: false, defaultValue: 7, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", description: "Multiplier for the volatility calculation", isRequired: false, defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddResult("Saf", "Stop and Follow", ResultType.Default, isDefault: true)
            .AddResult("IsStop", "Is Stop", ResultType.Default, isDefault: false)
            .Build();
}