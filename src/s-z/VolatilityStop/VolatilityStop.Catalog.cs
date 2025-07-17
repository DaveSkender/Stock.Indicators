namespace Skender.Stock.Indicators;

public static partial class VolatilityStop
{
    // Volatility Stop Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Volatility Stop")
            .WithId("VOL-STOP")
            .WithStyle(Style.Series)
            .WithCategory(Category.StopAndReverse)
            .WithMethodName("ToVolatilityStop")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the volatility calculation", isRequired: false, defaultValue: 7, minimum: 1, maximum: 50)
            .AddParameter<double>("multiplier", "Multiplier", description: "Multiplier for the volatility calculation", isRequired: false, defaultValue: 3.0, minimum: 0.1, maximum: 10.0)
            .AddResult("Sar", "Stop and Reverse", ResultType.Default, isReusable: true)
            .AddResult("IsStop", "Is Stop", ResultType.Default)
            .Build();
}
