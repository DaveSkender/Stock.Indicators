namespace Skender.Stock.Indicators;

public static partial class Dpo
{
    /// <summary>
    /// DPO Common Base Listing
    /// </summary>
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

    /// <summary>
    /// DPO Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// DPO BufferList Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // Note: DPO StreamHub is not feasible due to lookahead requirements.
    // DPO requires future SMA values to calculate detrended prices,
    // making real-time streaming implementation fundamentally incompatible
    // with the StreamHub architecture which expects results for each input.
}
