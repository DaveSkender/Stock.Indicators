namespace Skender.Stock.Indicators;

public static partial class ForceIndex
{
    // FORCE Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Force Index")
            .WithId("FORCE")
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToForceIndex")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 2, minimum: 1, maximum: 250)
            .AddResult("ForceIndex", "Force Index", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for FORCE.
    // No BufferListing for FORCE.
}
