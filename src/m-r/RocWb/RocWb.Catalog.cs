namespace Skender.Stock.Indicators;

public static partial class RocWb
{
    // ROC with Bands Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("ROC with Bands")
            .WithId("ROC-WB")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToRocWb")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the ROC calculation", isRequired: false, defaultValue: 20, minimum: 1, maximum: 250)
            .AddParameter<int>("emaPeriods", "EMA Periods", description: "Number of periods for the EMA calculation", isRequired: false, defaultValue: 5, minimum: 1, maximum: 100)
            .AddParameter<int>("stdDevPeriods", "Standard Deviation Periods", description: "Number of periods for the standard deviation calculation", isRequired: false, defaultValue: 5, minimum: 1, maximum: 100)
            .AddResult("Roc", "ROC", ResultType.Default, isReusable: true)
            .AddResult("RocEma", "ROC EMA", ResultType.Default)
            .AddResult("UpperBand", "Upper Band", ResultType.Default)
            .AddResult("LowerBand", "Lower Band", ResultType.Default)
            .Build();
}
