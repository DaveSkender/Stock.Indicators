namespace Skender.Stock.Indicators;

public static partial class Ema
{
    /// <summary>
    /// Catalog listing for the Exponential Moving Average (EMA) indicator.
    /// </summary>
    public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA")
        .WithStyle(Style.Series)
        .WithCategory(Category.MovingAverage)
        .AddParameter<int>("lookbackPeriods", "Lookback Period",
            description: "Number of periods for the EMA calculation",
            isRequired: true,
            defaultValue: 20,
            minimum: 2,
            maximum: 250)
        .AddParameter<decimal?>("smoothingFactor", "Smoothing Factor",
            description: "Optional custom smoothing factor")
        .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)
        .Build();
}
