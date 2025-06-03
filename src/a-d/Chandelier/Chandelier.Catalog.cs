// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators; // This namespace should contain Direction enum

namespace Skender.Stock.Indicators;

public static partial class Chandelier
{
    // CHEXIT Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Chandelier Exit") // From catalog.bak.json
            .WithId("CHEXIT") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.StopAndReverse) // From catalog.bak.json Category: "StopAndReverse"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 22, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 3.0, minimum: 1.0, maximum: 10.0) // From catalog.bak.json
            .AddEnumParameter<Direction>("type", "Direction", defaultValue: Direction.Long) // Use AddEnumParameter; Direction.Long corresponds to 0
            .AddResult("ChandelierExit", "Chandelier Exit", ResultType.Default, isDefault: true) // From ChandelierResult model
            .Build();

    // No StreamListing for CHEXIT.
    // No BufferListing for CHEXIT.
}
