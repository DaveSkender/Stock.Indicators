namespace Skender.Stock.Indicators;

public static partial class Correlation
{
    // CORR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Correlation")
            .WithId("CORR")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddSeriesParameter("sourceA", "Source A")
            .AddSeriesParameter("sourceB", "Source B")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("VarianceA", "Variance A", ResultType.Default, isDefault: false)
            .AddResult("VarianceB", "Variance B", ResultType.Default, isDefault: false)
            .AddResult("Covariance", "Covariance", ResultType.Default, isDefault: false)
            .AddResult("Correlation", "Correlation", ResultType.Default, isDefault: true)
            .AddResult("RSquared", "R-Squared", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for CORR.
    // No BufferListing for CORR.
}
