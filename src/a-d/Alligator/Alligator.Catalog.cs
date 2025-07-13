namespace Skender.Stock.Indicators;

public static partial class Alligator
{
    // Alligator Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Williams Alligator")
            .WithId("ALLIGATOR")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("jawPeriods", "Jaw Periods", description: "Lookback periods for the Jaw line", isRequired: true, defaultValue: 13, minimum: 2, maximum: 250)
            .AddParameter<int>("jawOffset", "Jaw Offset", description: "Offset periods for the Jaw line", isRequired: true, defaultValue: 8, minimum: 1, maximum: 50)
            .AddParameter<int>("teethPeriods", "Teeth Periods", description: "Lookback periods for the Teeth line", isRequired: true, defaultValue: 8, minimum: 2, maximum: 250)
            .AddParameter<int>("teethOffset", "Teeth Offset", description: "Offset periods for the Teeth line", isRequired: true, defaultValue: 5, minimum: 1, maximum: 50)
            .AddParameter<int>("lipsPeriods", "Lips Periods", description: "Lookback periods for the Lips line", isRequired: true, defaultValue: 5, minimum: 2, maximum: 250)
            .AddParameter<int>("lipsOffset", "Lips Offset", description: "Offset periods for the Lips line", isRequired: true, defaultValue: 3, minimum: 1, maximum: 50)
            .AddResult("Jaw", "Jaw", ResultType.Default, isDefault: false)
            .AddResult("Teeth", "Teeth", ResultType.Default, isDefault: false)
            .AddResult("Lips", "Lips", ResultType.Default, isDefault: true)
            .Build();

    // Alligator Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Williams Alligator")
            .WithId("ALLIGATOR")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceTrend)
            .AddParameter<int>("jawPeriods", "Jaw Periods", description: "Lookback periods for the Jaw line", isRequired: true, defaultValue: 13, minimum: 2, maximum: 250)
            .AddParameter<int>("jawOffset", "Jaw Offset", description: "Offset periods for the Jaw line", isRequired: true, defaultValue: 8, minimum: 1, maximum: 50)
            .AddParameter<int>("teethPeriods", "Teeth Periods", description: "Lookback periods for the Teeth line", isRequired: true, defaultValue: 8, minimum: 2, maximum: 250)
            .AddParameter<int>("teethOffset", "Teeth Offset", description: "Offset periods for the Teeth line", isRequired: true, defaultValue: 5, minimum: 1, maximum: 50)
            .AddParameter<int>("lipsPeriods", "Lips Periods", description: "Lookback periods for the Lips line", isRequired: true, defaultValue: 5, minimum: 2, maximum: 250)
            .AddParameter<int>("lipsOffset", "Lips Offset", description: "Offset periods for the Lips line", isRequired: true, defaultValue: 3, minimum: 1, maximum: 50)
            .AddResult("Jaw", "Jaw", ResultType.Default, isDefault: false)
            .AddResult("Teeth", "Teeth", ResultType.Default, isDefault: false)
            .AddResult("Lips", "Lips", ResultType.Default, isDefault: true)
            .Build();
}
