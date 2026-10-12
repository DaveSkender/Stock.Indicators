namespace FacioQuo.Stock.Indicators;

public static partial class Rsi
{
    /// <summary>
    /// RSI Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Relative Strength Index")
            .WithId("RSI")
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the RSI calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult(nameof(RsiResult.Rsi), "RSI", "Rsi", ResultType.Default, isReusable: true)
            .WithPane("Rsi", 0, 100, 70, 30)
            .Build();

    /// <summary>
    /// RSI Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName(typeof(Rsi), "ToRsi")
            .Build();

    /// <summary>
    /// RSI Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .WithMethodName(typeof(Rsi), "ToRsiHub")
            .Build();

    /// <summary>
    /// RSI Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .WithMethodName(typeof(Rsi), "ToRsiList")
            .Build();
}
