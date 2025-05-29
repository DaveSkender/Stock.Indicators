namespace Skender.Stock.Indicators;

public static partial class Ema
{
    /// <summary>
    /// Catalog listing for the Exponential Moving Average (EMA) Series indicator.
    /// </summary>
    public static readonly IndicatorListing SeriesListing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA-Series")
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

    /// <summary>
    /// Catalog listing for the Exponential Moving Average (EMA) Stream indicator.
    /// </summary>
    public static readonly IndicatorListing StreamListing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA-Stream")
        .WithStyle(Style.Stream)
        .WithCategory(Category.MovingAverage)
        .AddParameter<int>("lookbackPeriods", "Lookback Period",
            description: "Number of periods for the EMA calculation",
            isRequired: true,
            defaultValue: 20,
            minimum: 2,
            maximum: 250)
        .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)
        .Build();

    /// <summary>
    /// Catalog listing for the Exponential Moving Average (EMA) Buffer indicator.
    /// </summary>
    public static readonly IndicatorListing BufferListing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA-Buffer")
        .WithStyle(Style.Buffer)
        .WithCategory(Category.MovingAverage)
        .AddParameter<int>("lookbackPeriods", "Lookback Period",
            description: "Number of periods for the EMA calculation",
            isRequired: true,
            defaultValue: 20,
            minimum: 2,
            maximum: 250)
        .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)
        .Build();
}
