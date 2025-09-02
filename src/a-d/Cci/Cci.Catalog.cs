namespace Skender.Stock.Indicators;

public static partial class Cci
{
    // CCI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Commodity Channel Index (CCI)")
            .WithId("CCI")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToCci")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Cci", "CCI", ResultType.Default, isReusable: true)
            .Build();

    // No StreamListing for CCI.
    // No BufferListing for CCI.
}
