// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Cmf
{
    // CMF Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Chaikin Money Flow (CMF)") // From catalog.bak.json
            .WithId("CMF") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased) // From catalog.bak.json Category: "VolumeBased"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("MoneyFlowMultiplier", "Money Flow Multiplier", ResultType.Default, isDefault: false) // From CmfResult model
            .AddResult("MoneyFlowVolume", "Money Flow Volume", ResultType.Default, isDefault: false) // From CmfResult model
            .AddResult("Cmf", "CMF", ResultType.Default, isDefault: true) // From CmfResult model
            .Build();

    // No StreamListing for CMF.
    // No BufferListing for CMF.
}
