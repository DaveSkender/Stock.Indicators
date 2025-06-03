using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Ema
{
    // EMA Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Exponential Moving Average") // From catalog.bak.json
            .WithId("EMA") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage) // From catalog.bak.json Category: "MovingAverage"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddResult("Ema", "EMA", ResultType.Default, isDefault: true) // From EmaResult model
            .Build();

    // EMA Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Exponential Moving Average (Stream)") // Adjusted name
            .WithId("EMA")
            .WithStyle(Style.Stream)
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)
            .Build();

    // EMA Buffer Listing
    public static readonly IndicatorListing BufferListing =
        new IndicatorListingBuilder()
            .WithName("Exponential Moving Average (Buffer)") // Adjusted name
            .WithId("EMA")
            .WithStyle(Style.Buffer)
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)
            .Build();
}
