namespace Skender.Stock.Indicators;

public static partial class Vwma
{
    // Volume Weighted Moving Average Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Volume Weighted Moving Average")
            .WithId("VWMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the VWMA calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Vwma", "VWMA", ResultType.Default, isDefault: true)
            .Build();
}