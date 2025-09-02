namespace Skender.Stock.Indicators;

public static partial class Dpo
{
    // DPO Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Detrended Price Oscillator")
            .WithId("DPO")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToDpo")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Dpo", "DPO", ResultType.Default, isReusable: true)
            .AddResult("Sma", "SMA", ResultType.Default)
            .Build();

    // No StreamListing for DPO.
    // No BufferListing for DPO.
}
