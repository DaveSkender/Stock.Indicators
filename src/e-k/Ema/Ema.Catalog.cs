namespace Skender.Stock.Indicators;

public static partial class Ema
{
    // EMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Exponential Moving Average")
            .WithId("EMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToEma")
            .AddParameter<int>("lookbackPeriods", "Lookback Period", description: "Number of periods for the EMA calculation", isRequired: true, defaultValue: 20, minimum: 2, maximum: 250)
            .AddParameter<double?>("smoothingFactor", "Smoothing Factor", description: "Optional custom smoothing factor", isRequired: false, defaultValue: null) // Optional parameter
            .AddResult("Ema", "EMA", ResultType.Default, isReusable: true)
            .Build();

    // EMA Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Exponential Moving Average") // Adjusted name
            .WithId("EMA")
            .WithStyle(Style.Stream)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToEma")
            .AddParameter<int>("lookbackPeriods", "Lookback Period", description: "Number of periods for the EMA calculation", isRequired: true, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Ema", "EMA", ResultType.Default, isReusable: true)
            .Build();

    // EMA Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new IndicatorListingBuilder()
            .WithName("Exponential Moving Average") // Adjusted name
            .WithId("EMA")
            .WithStyle(Style.Buffer)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToEma")
            .AddParameter<int>("lookbackPeriods", "Lookback Period", description: "Number of periods for the EMA calculation", isRequired: true, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Ema", "EMA", ResultType.Default, isReusable: true)
            .Build();
}
