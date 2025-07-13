namespace Skender.Stock.Indicators;

public static partial class Alma
{
    // ALMA Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Arnaud Legoux Moving Average (ALMA)")
            .WithId("ALMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 9, minimum: 2, maximum: 250)
            .AddParameter<double>("offset", "Offset", defaultValue: 0.85, minimum: 0.0, maximum: 1.0)
            .AddParameter<double>("sigma", "Sigma", defaultValue: 6.0, minimum: 0.1, maximum: 10.0)
            .AddResult("Alma", "Arnaud Legoux Moving Average (ALMA)", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for ALMA.
    // No BufferListing for ALMA.
}
