namespace Skender.Stock.Indicators;

public static partial class Prs
{
    // Price Relative Strength Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Price Relative Strength")
            .WithId("PRS")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToPrs")
            .AddSeriesParameter("sourceEval", "Source Evaluated", description: "Source data to be evaluated")
            .AddSeriesParameter("sourceBase", "Source Base", description: "Base source data for comparison")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the PRS calculation", isRequired: false, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Prs", "PRS", ResultType.Default, isDefault: true)
            .AddResult("PrsPercent", "PRS %", ResultType.Default, isDefault: false)
            .AddResult("Sma", "SMA", ResultType.Default, isDefault: false)
            .Build();
}
