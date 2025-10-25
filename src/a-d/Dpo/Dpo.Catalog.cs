namespace Skender.Stock.Indicators;

public static partial class Dpo
{
    // DPO Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Detrended Price Oscillator")
            .WithId("DPO")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToDpo")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Dpo", "DPO", ResultType.Default, isReusable: true)
            .AddResult("Sma", "SMA", ResultType.Default)
            .Build();

    // DPO Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // DPO BufferList Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // DPO StreamHub Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();
}
