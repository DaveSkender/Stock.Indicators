// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Cmo
{
    // CMO Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Chande Momentum Oscillator") // From catalog.bak.json
            .WithId("CMO") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator) // From catalog.bak.json Category: "Oscillator"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("Cmo", "CMO", ResultType.Default, isDefault: true) // From CmoResult model
            .Build();

    // No StreamListing for CMO.
    // No BufferListing for CMO.
}
