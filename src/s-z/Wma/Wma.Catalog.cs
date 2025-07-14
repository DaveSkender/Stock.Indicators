namespace Skender.Stock.Indicators;

public static partial class Wma
{
    // Weighted Moving Average Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Weighted Moving Average")
            .WithId("WMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToWma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the WMA calculation", isRequired: true, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Wma", "WMA", ResultType.Default, isDefault: true)
            .Build();
}
