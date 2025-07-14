namespace Skender.Stock.Indicators;

public static partial class Gator
{
    // GATOR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Gator Oscillator")
            .WithId("GATOR")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToGator")
            .AddResult("Upper", "Upper", ResultType.Default)
            .AddResult("Lower", "Lower", ResultType.Default)
            .AddResult("UpperIsExpanding", "Upper Is Expanding", ResultType.Default)
            .AddResult("LowerIsExpanding", "Lower Is Expanding", ResultType.Default)
            .Build();

    // No StreamListing for GATOR.
    // No BufferListing for GATOR.
}
