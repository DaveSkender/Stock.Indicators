namespace Skender.Stock.Indicators;

public static partial class Gator
{
    // GATOR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Gator Oscillator")
            .WithId("GATOR")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToGator")
            .AddResult("Upper", "Upper", ResultType.Default, false)
            .AddResult("Lower", "Lower", ResultType.Default, false)
            .AddResult("UpperIsExpanding", "Upper Is Expanding", ResultType.Default, false)
            .AddResult("LowerIsExpanding", "Lower Is Expanding", ResultType.Default, false)
            .Build();

    // No StreamListing for GATOR.
    // No BufferListing for GATOR.
}
