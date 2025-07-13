namespace Skender.Stock.Indicators;

public static partial class Gator
{
    // GATOR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Gator Oscillator") // From catalog.bak.json
            .WithId("GATOR") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator) // From catalog.bak.json Category: "Oscillator"
                                               // No parameters for GATOR in catalog.bak.json
            .AddResult("Upper", "Upper", ResultType.Default, isDefault: false) // From GatorResult model
            .AddResult("Lower", "Lower", ResultType.Default, isDefault: false) // From GatorResult model
            .AddResult("UpperIsExpanding", "Upper Is Expanding", ResultType.Default, isDefault: false) // From GatorResult model
            .AddResult("LowerIsExpanding", "Lower Is Expanding", ResultType.Default, isDefault: false) // From GatorResult model
            .Build();

    // No StreamListing for GATOR.
    // No BufferListing for GATOR.
}
