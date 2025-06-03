using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class ElderRay
{
    // ELDER-RAY Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Elder-ray Index") // From catalog.bak.json
            .WithId("ELDER-RAY") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend) // From catalog.bak.json Category: "PriceTrend"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 13, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("Ema", "EMA", ResultType.Default, isDefault: false) // From ElderRayResult model
            .AddResult("BullPower", "Bull Power", ResultType.Default, isDefault: false) // From ElderRayResult model
            .AddResult("BearPower", "Bear Power", ResultType.Default, isDefault: false) // From ElderRayResult model
            .Build();

    // No StreamListing for ELDER-RAY.
    // No BufferListing for ELDER-RAY.
}
