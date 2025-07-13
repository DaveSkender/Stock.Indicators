namespace Skender.Stock.Indicators;

public static partial class Bop
{
    // BOP Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Balance of Power (BOP)")
            .WithId("BOP")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("smoothPeriods", "Smooth Periods", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Bop", "BOP", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for BOP.
    // No BufferListing for BOP.
}
