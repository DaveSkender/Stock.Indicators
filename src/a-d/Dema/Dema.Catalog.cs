using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Dema
{
    // DEMA Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Double Exponential Moving Average") // From catalog.bak.json
            .WithId("DEMA") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage) // From catalog.bak.json Category: "MovingAverage"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddResult("Dema", "DEMA", ResultType.Default, isDefault: true) // From DemaResult model
            .Build();

    // No StreamListing for DEMA.
    // No BufferListing for DEMA.
}
