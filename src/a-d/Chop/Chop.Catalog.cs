// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Chop
{
    // CHOP Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Choppiness Index") // From catalog.bak.json
            .WithId("CHOP") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator) // From catalog.bak.json Category: "Oscillator"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("Chop", "CHOP", ResultType.Default, isDefault: true) // From ChopResult model
            .Build();

    // No StreamListing for CHOP.
    // No BufferListing for CHOP.
}
