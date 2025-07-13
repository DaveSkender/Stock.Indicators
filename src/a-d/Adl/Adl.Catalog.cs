namespace Skender.Stock.Indicators;

public static partial class Adl
{
    // ADL Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Accumulation Distribution Line (ADL)")
            .WithId("ADL")
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased)
            .AddResult("Adl", "Accumulation Distribution Line (ADL)", ResultType.Default, isDefault: true)
            .Build();

    // ADL Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Accumulation Distribution Line (ADL) (Stream)")
            .WithId("ADL")
            .WithStyle(Style.Stream)
            .WithCategory(Category.VolumeBased)
            .AddResult("Adl", "Accumulation Distribution Line (ADL)", ResultType.Default, isDefault: true)
            .Build();

    // No BufferListing for ADL.
}
