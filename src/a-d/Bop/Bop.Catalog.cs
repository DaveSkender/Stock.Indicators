namespace Skender.Stock.Indicators;

public static partial class Bop
{
    /// <summary>
    /// BOP Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Balance of Power (BOP)")
            .WithId("BOP")
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("smoothPeriods", "Smooth Periods", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Bop", "BOP", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// BOP Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToBop")
            .Build();

    // No StreamListing for BOP.
    // No BufferListing for BOP.
}
