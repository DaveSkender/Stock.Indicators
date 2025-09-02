namespace Skender.Stock.Indicators;

public static partial class Alma
{
    // ALMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Arnaud Legoux Moving Average (ALMA)")
            .WithId("ALMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToAlma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 9, minimum: 2, maximum: 250)
            .AddParameter<double>("offset", "Offset", defaultValue: 0.85, minimum: 0.0, maximum: 1.0)
            .AddParameter<double>("sigma", "Sigma", defaultValue: 6.0, minimum: 0.1, maximum: 10.0)
            .AddResult("Alma", "Arnaud Legoux Moving Average (ALMA)", ResultType.Default, isReusable: true)
            .Build();

    // No StreamListing for ALMA.
    // No BufferListing for ALMA.
}
