namespace Skender.Stock.Indicators;

public static partial class ConnorsRsi
{
    // CRSI Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("ConnorsRSI (CRSI)")
            .WithId("CRSI")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToConnorsRsi")
            .AddParameter<int>("rsiPeriods", "RSI Periods", defaultValue: 3, minimum: 2, maximum: 250)
            .AddParameter<int>("streakPeriods", "Streak Periods", defaultValue: 2, minimum: 2, maximum: 50)
            .AddParameter<int>("rankPeriods", "Rank Periods", defaultValue: 100, minimum: 2, maximum: 250)
            .AddResult("Streak", "Streak", ResultType.Default)
            .AddResult("Rsi", "RSI", ResultType.Default)
            .AddResult("RsiStreak", "RSI of Streak", ResultType.Default)
            .AddResult("PercentRank", "Percent Rank", ResultType.Default)
            .AddResult("ConnorsRsi", "ConnorsRSI", ResultType.Default, isReusable: true)
            .Build();

    // CRSI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // CRSI Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // CRSI Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();
}
