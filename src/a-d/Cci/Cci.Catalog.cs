namespace Skender.Stock.Indicators;

public static partial class Cci
{
    // CCI Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Commodity Channel Index (CCI)")
            .WithId("CCI")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToCci")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Cci", "CCI", ResultType.Default, isReusable: true)
            .Build();

    // CCI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // CCI Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // CCI Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
