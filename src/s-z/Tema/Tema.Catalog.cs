namespace Skender.Stock.Indicators;

public static partial class Tema
{
    // Triple Exponential Moving Average Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Triple Exponential Moving Average")
            .WithId("TEMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToTema")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the TEMA calculation", isRequired: false, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Tema", "TEMA", ResultType.Default, isDefault: true)
            .Build();
}
