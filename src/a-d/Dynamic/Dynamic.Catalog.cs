namespace Skender.Stock.Indicators;

public static partial class MgDynamic
{
    // Dynamic Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("McGinley Dynamic")
            .WithId("DYNAMIC")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the McGinley Dynamic calculation", isRequired: true, defaultValue: 14, minimum: 1, maximum: 250)
            .AddParameter<double>("kFactor", "K Factor", description: "Smoothing factor for the calculation", isRequired: false, defaultValue: 0.6, minimum: 0.1, maximum: 2.0)
            .AddResult("Dynamic", "McGinley Dynamic", ResultType.Default, isDefault: true)
            .Build();
}
