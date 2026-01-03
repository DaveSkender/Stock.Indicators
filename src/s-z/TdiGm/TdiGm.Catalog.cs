namespace Skender.Stock.Indicators;

public static partial class TdiGm
{
    /// <summary>
    /// TDI-GM Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Traders Dynamic Index (Goldminds)")
            .WithId("TDIGM")
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("rsiPeriod", "RSI Period", description: "Number of periods for the RSI calculation", isRequired: false, defaultValue: 21, minimum: 1, maximum: 250)
            .AddParameter<int>("bandLength", "Band Length", description: "Number of periods for band calculations", isRequired: false, defaultValue: 34, minimum: 1, maximum: 250)
            .AddParameter<int>("fastLength", "Fast Length", description: "Number of periods for fast moving average", isRequired: false, defaultValue: 2, minimum: 1, maximum: 250)
            .AddParameter<int>("slowLength", "Slow Length", description: "Number of periods for slow moving average", isRequired: false, defaultValue: 7, minimum: 1, maximum: 250)
            .AddResult("Upper", "Upper Band", ResultType.Default)
            .AddResult("Lower", "Lower Band", ResultType.Default)
            .AddResult("Middle", "Middle Band", ResultType.Default)
            .AddResult("Fast", "Fast MA", ResultType.Default, isReusable: true)
            .AddResult("Slow", "Slow MA", ResultType.Default)
            .Build();

    /// <summary>
    /// TDI-GM Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToTdiGm")
            .Build();

    /// <summary>
    /// TDI-GM Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .WithMethodName("ToTdiGmHub")
            .Build();

    /// <summary>
    /// TDI-GM Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .WithMethodName("ToTdiGmList")
            .Build();
}
