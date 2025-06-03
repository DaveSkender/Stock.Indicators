using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class Fcb
{
    // FCB Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Fractal Chaos Bands") // From catalog.bak.json
            .WithId("FCB") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel) // From catalog.bak.json Category: "PriceChannel"
            .AddParameter<int>("windowSpan", "Window Span", defaultValue: 2, minimum: 2, maximum: 30) // From catalog.bak.json
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: false) // From FcbResult model
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false) // From FcbResult model
            .Build();

    // No StreamListing for FCB.
    // No BufferListing for FCB.
}
