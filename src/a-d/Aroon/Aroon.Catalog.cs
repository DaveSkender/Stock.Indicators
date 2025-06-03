// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Aroon
{
    // AROON Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Aroon Up/Down") // From catalog.bak.json
            .WithId("AROON") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend) // From catalog.bak.json Category: "PriceTrend"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 25, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("AroonUp", "Aroon Up", ResultType.Default, isDefault: false) // From AroonResult model
            .AddResult("AroonDown", "Aroon Down", ResultType.Default, isDefault: false) // From AroonResult model
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isDefault: true) // From AroonResult model, making Oscillator default
            .Build();

    // No StreamListing for AROON.
    // No BufferListing for AROON.
}
