using System.Reflection;

namespace Catalogging;

/// <summary>
/// Runs every indicator in each style it lists and requires the Buffer and Stream results
/// to equal the Series results exactly, at the catalog defaults and at the minimum and the
/// next two values of each integer parameter.
/// </summary>
/// <remarks>
/// Per-indicator tests compare styles at one or two parameter sets, usually the defaults.
/// Warmup and window-edge defects show at short periods, where buffers fill on the first
/// bars. Results are compared by public property values, since a record's equality also
/// sees its non-public state. A parameter set the Series rejects, such as a minimum that
/// conflicts with another parameter's default, is skipped; the defaults never are. A family
/// with integer parameters fails when every non-default set is skipped. Double
/// and enum parameters stay at their defaults, and hub rollback and late arrival are left
/// to the per-indicator tests.
/// </remarks>
[TestClass]
public class CatalogStyleParityTests : TestBase
{
    private const int ValuesPerParameter = 3;

    [TestMethod]
    public void BufferAndStream_MatchSeries_AtShortPeriods()
    {
        List<string> failures = [];
        HashSet<Style> stylesCompared = [];

        foreach (IGrouping<string, IndicatorListing> family in Catalog.Get().GroupBy(static l => l.Uiid))
        {
            IndicatorListing series = family.FirstOrDefault(static l => l.Style == Style.Series);

            if (series is null)
            {
                failures.Add($"{family.Key}: no Series listing to compare against");
                continue;
            }

            int nonDefaultSets = 0;
            int nonDefaultCompared = 0;

            foreach (Dictionary<string, object> args in ParameterSets(series))
            {
                if (args.Count > 0)
                {
                    nonDefaultSets++;
                }

                List<object> expected;

                try
                {
                    expected = Execute(series, args);
                }
                catch (TargetInvocationException ex) when (ex.InnerException is ArgumentException && args.Count > 0)
                {
                    continue;
                }

                if (args.Count > 0)
                {
                    nonDefaultCompared++;
                }

                foreach (IndicatorListing other in family.Where(static l => l.Style != Style.Series))
                {
                    stylesCompared.Add(other.Style);
                    string label = $"{CatalogReflection.Describe(other)} {Format(args)}";
                    List<object> actual;

                    try
                    {
                        actual = Execute(other, args);
                    }
                    catch (TargetInvocationException ex)
                    {
                        failures.Add($"{label}: threw {ex.InnerException?.GetType().Name}: {ex.InnerException?.Message}");
                        continue;
                    }

                    if (actual.Count != expected.Count)
                    {
                        failures.Add($"{label}: {actual.Count} results, Series has {expected.Count}");
                        continue;
                    }

                    int first = Enumerable.Range(0, expected.Count)
                        .FirstOrDefault(i => !PublicValues(expected[i]).SequenceEqual(PublicValues(actual[i])), -1);

                    if (first >= 0)
                    {
                        failures.Add($"{label}: first differs at [{first}] {Difference(expected[first], actual[first])}");
                    }
                }
            }

            if (nonDefaultSets > 0 && nonDefaultCompared == 0)
            {
                failures.Add($"{family.Key}: Series rejected all {nonDefaultSets} non-default sets, so only defaults were compared");
            }
        }

        failures.AddRange(new[] { Style.Buffer, Style.Stream }
            .Where(st => !stylesCompared.Contains(st))
            .Select(static st => $"{st}: no listing in this style was compared"));

        if (failures.Count > 0)
        {
            Assert.Fail($"Styles disagree with Series:\n{string.Join("\n", failures)}");
        }
    }

    /// <summary>
    /// The catalog defaults, then each integer parameter alone at its minimum and the next
    /// two values above it.
    /// </summary>
    private static IEnumerable<Dictionary<string, object>> ParameterSets(IndicatorListing listing)
    {
        yield return new(StringComparer.Ordinal);

        foreach (IndicatorParam param in listing.Parameters ?? [])
        {
            if (param.DataType != "Int32" || param.Minimum is not double min)
            {
                continue;
            }

            for (int step = 0; step < ValuesPerParameter; step++)
            {
                yield return new(StringComparer.Ordinal) { [param.ParameterName] = (int)min + step };
            }
        }
    }

    private static List<object> Execute(IndicatorListing listing, Dictionary<string, object> overrides)
    {
        Dictionary<string, object> args = new(overrides, StringComparer.Ordinal);

        if (listing.Style == Style.Stream)
        {
            return CatalogReflection.ExecuteHub(listing, args, Bars);
        }

        foreach (IndicatorParam series in (listing.Parameters ?? [])
            .Where(static p => p.DataType == IndicatorParam.SeriesDataType))
        {
            args[series.ParameterName] = Bars.Cast<IReusable>().ToList();
        }

        return [.. ListingExecutor.Execute<object>(Bars, listing, args)];
    }

    private static IEnumerable<object> PublicValues(object result)
        => PublicProperties(result).Select(p => p.GetValue(result));

    private static IOrderedEnumerable<PropertyInfo> PublicProperties(object result)
        => result.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(static p => p.Name, StringComparer.Ordinal);

    private static string Difference(object expected, object actual)
        => string.Join("; ", PublicProperties(expected)
            .Where(p => !Equals(p.GetValue(expected), p.GetValue(actual)))
            .Select(p => $"{p.Name}: Series {p.GetValue(expected)}, got {p.GetValue(actual)}"));

    private static string Format(Dictionary<string, object> args)
        => args.Count == 0
            ? "(defaults)"
            : string.Join(", ", args.Select(static kv => $"{kv.Key}={kv.Value}"));
}
