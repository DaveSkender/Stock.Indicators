namespace Skender.Stock.Indicators;

public static partial class Prs
{
    // Price Relative Strength Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Price Relative Strength")
            .WithId("PRS")
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToPrs")
            .AddSeriesParameter("sourceEval", "Source Evaluated", description: "Source data to be evaluated")
            .AddSeriesParameter("sourceBase", "Source Base", description: "Base source data for comparison")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the PRS calculation", isRequired: false, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Prs", "PRS", ResultType.Default, isReusable: true)
            .AddResult("PrsPercent", "PRS %", ResultType.Default)
            .AddResult("Sma", "SMA", ResultType.Default)
            .Build();

    // Price Relative Strength Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for PRS.
    // No BufferListing for PRS.
}
