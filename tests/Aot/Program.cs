using FacioQuo.Stock.Indicators;

// Runs every catalog listing and prints one line per outcome. A NativeAOT publish must
// print exactly what the JIT run prints; any difference is behavior trimming removed.

List<Bar> bars = [];
DateTime start = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

for (int i = 0; i < 500; i++)
{
    decimal close = 100m + (decimal)Math.Round(10 * Math.Sin(i / 7d) + i / 25d, 4);
    bars.Add(new Bar(start.AddDays(i), close - 0.5m, close + 1.25m, close - 1.5m, close, 1000m + (i % 17 * 50)));
}

int failures = 0;

foreach (IndicatorListing listing in Catalog.Get()
    .OrderBy(static l => l.Uiid, StringComparer.Ordinal)
    .ThenBy(static l => l.Style))
{
    string outcome;

    try
    {
        IReadOnlyList<object> results = listing.Execute<object>(bars);
        outcome = $"ok {results.Count}";
    }
    catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
    {
        outcome = $"rejected {ex.GetType().Name}: {ex.Message}";
    }

    if (listing.GetMethodOverloads().Count == 0)
    {
        outcome += " NO-OVERLOADS";
        failures++;
    }

    if (listing.ResultRecordType is null)
    {
        outcome += " NO-RESULT-RECORD-TYPE";
        failures++;
    }

    Console.WriteLine($"{listing.Uiid}/{listing.Style}: {listing.ResultRecordType} {outcome}");
}

return failures;
