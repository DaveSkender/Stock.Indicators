namespace Skender.Stock.Indicators;

public static partial class UlcerIndex
{
    // Ulcer Index Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Ulcer Index")
            .WithId("ULCER")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the Ulcer Index calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("UlcerIndex", "Ulcer Index", ResultType.Default, isDefault: true)
            .Build();
}
