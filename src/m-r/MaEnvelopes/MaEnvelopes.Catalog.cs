namespace Skender.Stock.Indicators;

public static partial class MaEnvelopes
{
    // MA-ENV Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Moving Average Envelopes")
            .WithId("MA-ENV")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToMaEnvelopes")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddParameter<double>("percentOffset", "Percent Offset", defaultValue: 2.5, minimum: 0.1, maximum: 10.0)
            .AddEnumParameter<MaType>("movingAverageType", "Moving Average Type", defaultValue: MaType.SMA) // MaType.SMA corresponds to 7 from JSON
            .AddResult("Centerline", "Centerline", ResultType.Default, isDefault: true)
            .AddResult("UpperEnvelope", "Upper Envelope", ResultType.Default, isDefault: false)
            .AddResult("LowerEnvelope", "Lower Envelope", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for MA-ENV.
    // No BufferListing for MA-ENV.
}
