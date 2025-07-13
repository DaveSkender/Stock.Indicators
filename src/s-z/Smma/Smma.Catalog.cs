namespace Skender.Stock.Indicators;

public static partial class Smma
{
    // Smoothed Moving Average Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Smoothed Moving Average")
            .WithId("SMMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMMA calculation", isRequired: false, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Smma", "SMMA", ResultType.Default, isDefault: true)
            .Build();
}