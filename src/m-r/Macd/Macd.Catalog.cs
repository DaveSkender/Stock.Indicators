using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Macd
{
    // MACD Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Moving Average Convergence/Divergence") // From catalog.bak.json
            .WithId("MACD") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend) // From catalog.bak.json Category: "PriceTrend"
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 12, minimum: 1, maximum: 200) // From catalog.bak.json
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 26, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddParameter<int>("signalPeriods", "Signal Periods", defaultValue: 9, minimum: 1, maximum: 50) // From catalog.bak.json
            .AddResult("Macd", "MACD", ResultType.Default, isDefault: true) // From MacdResult model
            .AddResult("Signal", "Signal", ResultType.Default, isDefault: false) // From MacdResult model
            .AddResult("Histogram", "Histogram", ResultType.Default, isDefault: false) // From MacdResult model
            .AddResult("FastEma", "Fast EMA", ResultType.Default, isDefault: false) // From MacdResult model
            .AddResult("SlowEma", "Slow EMA", ResultType.Default, isDefault: false) // From MacdResult model
            .Build();

    // No StreamListing for MACD.
    // No BufferListing for MACD.
}
