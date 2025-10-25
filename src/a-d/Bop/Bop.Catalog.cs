namespace Skender.Stock.Indicators;

public static partial class Bop
{
    // BOP Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Balance of Power (BOP)")
            .WithId("BOP")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToBop")
            .AddParameter<int>("smoothPeriods", "Smooth Periods", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Bop", "BOP", ResultType.Default, isReusable: true)
            .Build();

    // BOP Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for BOP.
    // No BufferListing for BOP.
}
