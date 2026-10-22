namespace FacioQuo.Stock.Indicators;

public static partial class Ema
{
    /// <summary>
    /// EMA Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Exponential Moving Average")
            .WithId("EMA")
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the EMA calculation", isRequired: true, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult(nameof(EmaResult.Ema), "EMA", IndicatorResult.PricePane, ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// EMA Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName(typeof(Ema), "ToEma")
            .Build();

    /// <summary>
    /// EMA Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .WithMethodName(typeof(Ema), "ToEmaHub")
            .Build();

    /// <summary>
    /// EMA Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .WithMethodName(typeof(Ema), "ToEmaList")
            .Build();
}
