using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Keltner
{
    // KELTNER Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Keltner Channels") // From catalog.bak.json
            .WithId("KELTNER") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel) // From catalog.bak.json Category: "PriceChannel"
            .AddParameter<int>("emaPeriods", "EMA Periods", defaultValue: 20, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 2.0, minimum: 0.01, maximum: 10.0) // From catalog.bak.json
            .AddParameter<int>("atrPeriods", "ATR Periods", defaultValue: 10, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: false) // From KeltnerResult model
            .AddResult("Centerline", "Centerline", ResultType.Default, isDefault: true) // From KeltnerResult model
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false) // From KeltnerResult model
            .AddResult("Width", "Width", ResultType.Default, isDefault: false) // From KeltnerResult model
            .Build();

    // No StreamListing for KELTNER.
    // No BufferListing for KELTNER.
}
