using FacioQuo.Stock.Indicators;

// Runs every catalog listing and prints one line per outcome. A NativeAOT publish must
// print exactly what the JIT run prints; any difference is behavior trimming removed.
// Lines carry exception types, not messages: NativeAOT may strip resource strings.

List<Bar> bars = [];
DateTime start = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

for (int i = 0; i < 500; i++)
{
    decimal close = 100m + (decimal)Math.Round(10 * Math.Sin(i / 7d) + i / 25d, 4);
    bars.Add(new Bar(start.AddDays(i), close - 0.5m, close + 1.25m, close - 1.5m, close, 1000m + (i % 17 * 50)));
}

IEnumerable<IBar> source = bars;
int failures = 0;

// Must run before any Catalog member: a listing built by hand is never registered, so
// its method resolves only through the resolver's fallback that builds the catalog.
IndicatorListing handBuilt = new() {
    Uiid = "EMA",
    Name = "Exponential Moving Average",
    Category = Category.MovingAverage,
    MethodName = "ToEma",
    LegendTemplate = "EMA([P1])",
    Parameters = [new() { DisplayName = "Lookback Period", ParameterName = "lookbackPeriods", DataType = "Int32", IsRequired = true }],
    Results = [new() { DisplayName = "EMA", DataName = "Ema", DataType = ResultType.Default }]
};

Require("cold-start/EMA", () => handBuilt.WithParamValue("lookbackPeriods", 20).FromSource(source).Execute<object>().Count);

foreach (IndicatorListing listing in Catalog.Get()
    .OrderBy(static l => l.Uiid, StringComparer.Ordinal)
    .ThenBy(static l => l.Style))
{
    bool noOverloads = listing.GetMethodOverloads().Count == 0;

    if (listing.ResultRecordType is null || noOverloads)
    {
        failures++;
    }

    Report(
        $"{listing.Uiid}/{listing.Style}: {listing.ResultRecordType ?? "NO-RESULT-RECORD-TYPE"}{(noOverloads ? " NO-OVERLOADS" : "")}",
        () => listing.Execute<object>(source).Count);
}

// Builder paths the loop never takes: a second series, and an omitted parameter.
IReadOnlyList<IReusable> closes = bars.Use(CandlePart.Close);
IReadOnlyList<IReusable> opens = bars.Use(CandlePart.Open);

Require("two-series/CORR", () => Catalog.Get("CORR", Style.Series)!
    .FromSource(closes, "sourceA")
    .WithParamValue("sourceB", opens)
    .FromSource(source)
    .Execute<object>().Count);

Require("omitted/ALMA", () => Catalog.Get("ALMA", Style.Series)!
    .WithoutParam("sigma")
    .FromSource(source)
    .Execute<object>().Count);

return failures;

// a case that must execute; a regression rejects it under both runtimes, so the diff alone misses it
void Require(string label, Func<int> run)
{
    if (!Report(label, run))
    {
        failures++;
    }
}

static bool Report(string label, Func<int> run)
{
    string outcome;
    bool ok = false;

    try
    {
        outcome = $"ok {run()}";
        ok = true;
    }
    catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
    {
        outcome = $"rejected {ex.GetType().Name}";
    }

    Console.WriteLine($"{label} {outcome}");
    return ok;
}
