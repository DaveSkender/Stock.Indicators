using System;
using System.Collections.Generic;
using Skender.Stock.Indicators; // This namespace should contain MaType enum

namespace Skender.Stock.Indicators;

public static partial class MaEnvelopes
{
    // MA-ENV Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Moving Average Envelopes") // From catalog.bak.json
            .WithId("MA-ENV") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel) // From catalog.bak.json Category: "PriceChannel"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddParameter<double>("percentOffset", "Percent Offset", defaultValue: 2.5, minimum: 0.1, maximum: 10.0) // From catalog.bak.json
            .AddEnumParameter<MaType>("movingAverageType", "Moving Average Type", defaultValue: MaType.SMA) // MaType.SMA corresponds to 7 from JSON
            .AddResult("Centerline", "Centerline", ResultType.Default, isDefault: true) // From MaEnvelopeResult model
            .AddResult("UpperEnvelope", "Upper Envelope", ResultType.Default, isDefault: false) // From MaEnvelopeResult model
            .AddResult("LowerEnvelope", "Lower Envelope", ResultType.Default, isDefault: false) // From MaEnvelopeResult model
            .Build();

    // No StreamListing for MA-ENV.
    // No BufferListing for MA-ENV.
}
