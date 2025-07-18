namespace Skender.Stock.Indicators;

public static partial class T3
{
    // T3 Moving Average Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("T3 Moving Average")
            .WithId("T3")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToT3")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the T3 calculation", isRequired: false, defaultValue: 5, minimum: 1, maximum: 250)
            .AddParameter<double>("volumeFactor", "Volume Factor", description: "Volume factor for the T3 calculation", isRequired: false, defaultValue: 0.7, minimum: 0.0, maximum: 1.0)
            .AddResult("T3", "T3", ResultType.Default, isReusable: true)
            .Build();
}
