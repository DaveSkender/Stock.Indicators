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
            .AddResult("Upper", "Upper", ResultType.Default, isDefault: true)
            .AddResult("Lower", "Lower", ResultType.Default, isDefault: false)
            .AddResult("UpperIsExpanding", "Upper Is Expanding", ResultType.Default, isDefault: false)
            .AddResult("LowerIsExpanding", "Lower Is Expanding", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for GATOR.
    // No BufferListing for GATOR.
}
