namespace Skender.Stock.Indicators;

public static partial class Cmo
{
    // CMO Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Chande Momentum Oscillator")
            .WithId("CMO")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToCmo")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Cmo", "CMO", ResultType.Default, isReusable: true)
            .Build();

    // CMO Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for CMO.
    // No BufferListing for CMO.
}
