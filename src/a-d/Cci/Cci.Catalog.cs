// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Cci
{
    // CCI Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Commodity Channel Index (CCI)") // From catalog.bak.json
            .WithId("CCI") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator) // From catalog.bak.json Category: "Oscillator"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("Cci", "CCI", ResultType.Default, isDefault: true) // From CciResult model
            .Build();

    // No StreamListing for CCI.
    // No BufferListing for CCI.
}
