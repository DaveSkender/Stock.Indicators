// Copyright Skender Consortium. Licensed under the MIT License.
// See LICENSE.txt for details.

using System;
using System.Collections.Generic;
using Skender.Stock.Indicators;

namespace Skender.Stock.Indicators;

public static partial class ConnorsRsi
{
    // CRSI Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("ConnorsRSI (CRSI)") // From catalog.bak.json
            .WithId("CRSI") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator) // From catalog.bak.json Category: "Oscillator"
            .AddParameter<int>("rsiPeriods", "RSI Periods", defaultValue: 3, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddParameter<int>("streakPeriods", "Streak Periods", defaultValue: 2, minimum: 2, maximum: 50) // From catalog.bak.json
            .AddParameter<int>("rankPeriods", "Rank Periods", defaultValue: 100, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddResult("Streak", "Streak", ResultType.Default, isDefault: false) // From ConnorsRsiResult model
            .AddResult("Rsi", "RSI", ResultType.Default, isDefault: false) // From ConnorsRsiResult model
            .AddResult("RsiStreak", "RSI of Streak", ResultType.Default, isDefault: false) // From ConnorsRsiResult model
            .AddResult("PercentRank", "Percent Rank", ResultType.Default, isDefault: false) // From ConnorsRsiResult model
            .AddResult("ConnorsRsi", "ConnorsRSI", ResultType.Default, isDefault: true) // From ConnorsRsiResult model
            .Build();

    // No StreamListing for CRSI.
    // No BufferListing for CRSI.
}
