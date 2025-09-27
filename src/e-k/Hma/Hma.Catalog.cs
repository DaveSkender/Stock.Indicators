namespace Skender.Stock.Indicators;

public static partial class Hma
{
    // HMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Hull Moving Average")
            .WithId("HMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToHma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Hma", "HMA", ResultType.Default, isReusable: true)
            .Build();

    // HMA Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder()
            .WithName("Hull Moving Average")
            .WithId("HMA")
            .WithStyle(Style.Stream)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToHma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Hma", "HMA", ResultType.Default, isReusable: true)
            .Build();

    // HMA Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder()
            .WithName("Hull Moving Average")
            .WithId("HMA")
            .WithStyle(Style.Buffer)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToHma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Hma", "HMA", ResultType.Default, isReusable: true)
            .Build();
}
