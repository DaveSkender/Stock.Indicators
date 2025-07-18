namespace Skender.Stock.Indicators;

public static partial class Roc
{
    // Rate of Change Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Rate of Change")
            .WithId("ROC")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToRoc")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the ROC calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Roc", "ROC", ResultType.Default, isReusable: true)
            .Build();
}
