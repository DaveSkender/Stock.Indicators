namespace Skender.Stock.Indicators;

public static partial class Trix
{
    // TRIX Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Triple Exponential Moving Average Oscillator")
            .WithId("TRIX")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToTrix")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the TRIX calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Trix", "TRIX", ResultType.Default, isReusable: true)
            .Build();

    // TRIX Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for TRIX.
    // No BufferListing for TRIX.
}
