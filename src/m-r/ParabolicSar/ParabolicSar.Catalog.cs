namespace Skender.Stock.Indicators;

public static partial class ParabolicSar
{
    // Parabolic SAR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Parabolic SAR")
            .WithId("PSAR")
            .WithStyle(Style.Series)
            .WithCategory(Category.StopAndReverse)
            .WithMethodName("ToParabolicSar")
            .AddParameter<double>("accelerationStep", "Acceleration Step", description: "Acceleration step for the Parabolic SAR calculation", isRequired: false, defaultValue: 0.02, minimum: 0.01, maximum: 0.1)
            .AddParameter<double>("maxAccelerationFactor", "Max Acceleration Factor", description: "Maximum acceleration factor for the Parabolic SAR calculation", isRequired: false, defaultValue: 0.2, minimum: 0.1, maximum: 1.0)
            .AddResult("Sar", "Parabolic SAR", ResultType.Default, isReusable: true)
            .AddResult("IsReversal", "Is Reversal", ResultType.Default)
            .Build();
}
