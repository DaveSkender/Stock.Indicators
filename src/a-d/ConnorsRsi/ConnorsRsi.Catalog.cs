namespace Skender.Stock.Indicators;

public static partial class ConnorsRsi
{
    // CRSI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("ConnorsRSI (CRSI)")
            .WithId("CRSI")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("rsiPeriods", "RSI Periods", defaultValue: 3, minimum: 2, maximum: 250)
            .AddParameter<int>("streakPeriods", "Streak Periods", defaultValue: 2, minimum: 2, maximum: 50)
            .AddParameter<int>("rankPeriods", "Rank Periods", defaultValue: 100, minimum: 2, maximum: 250)
            .AddResult("Streak", "Streak", ResultType.Default, isDefault: false)
            .AddResult("Rsi", "RSI", ResultType.Default, isDefault: false)
            .AddResult("RsiStreak", "RSI of Streak", ResultType.Default, isDefault: false)
            .AddResult("PercentRank", "Percent Rank", ResultType.Default, isDefault: false)
            .AddResult("ConnorsRsi", "ConnorsRSI", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for CRSI.
    // No BufferListing for CRSI.
}
