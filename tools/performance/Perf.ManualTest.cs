using System.Reflection;

namespace Performance;

/// <summary>
/// Manual performance test for dynamically selected indicators.
/// Uses environment variables PERF_TEST_KEYWORD and PERF_TEST_PERIODS.
/// </summary>
[ShortRunJob]
public class ManualTest
{
    private static readonly string Keyword = Environment.GetEnvironmentVariable("PERF_TEST_KEYWORD") ?? "sma";
    private static readonly int Periods = int.TryParse(
        Environment.GetEnvironmentVariable("PERF_TEST_PERIODS"),
        out int p) ? p : 500000;

    private IReadOnlyList<Quote> quotes = Array.Empty<Quote>();
    private IndicatorListing seriesListing = null!;
    private IndicatorListing streamListing = null!;
    private IndicatorListing bufferListing = null!;
    private QuoteHub quoteHub = null!;

    [GlobalSetup]
    public void Setup()
    {
        quotes = Data.GetRandom(Periods);

        // Find indicator listings by keyword
        IReadOnlyCollection<IndicatorListing> matches = Catalog.Search(Keyword);

        if (matches.Count == 0)
        {
            throw new InvalidOperationException($"No indicators found matching keyword: {Keyword}");
        }

        // Get listings for each style
        IndicatorListing foundSeriesListing = matches.FirstOrDefault(x => x.Style == Style.Series);
        IndicatorListing foundStreamListing = matches.FirstOrDefault(x => x.Style == Style.Stream);
        IndicatorListing foundBufferListing = matches.FirstOrDefault(x => x.Style == Style.Buffer);

        if (foundSeriesListing is null || foundStreamListing is null || foundBufferListing is null)
        {
            string missing = string.Join(", ",
                new[]
                {
                    foundSeriesListing is null ? "Series" : null,
                    foundStreamListing is null ? "Stream" : null,
                    foundBufferListing is null ? "Buffer" : null
                }.Where(x => x is not null));

            throw new InvalidOperationException(
                $"Indicator keyword '{Keyword}' must match an indicator with all three styles. Missing: {missing}.");
        }

        seriesListing = foundSeriesListing;
        streamListing = foundStreamListing;
        bufferListing = foundBufferListing;

        quoteHub = new QuoteHub();

        // Populate quote hub for stream tests (not counted in benchmark time)
        quoteHub.Add(quotes);

        // Log configuration
        Console.WriteLine("Manual Performance Test Configuration:");
        Console.WriteLine($"  Keyword: {Keyword}");
        Console.WriteLine($"  Periods: {Periods:N0}");
        Console.WriteLine($"  Series: {seriesListing.MethodName}");
        Console.WriteLine($"  Stream: {streamListing.MethodName}");
        Console.WriteLine($"  Buffer: {bufferListing.MethodName}");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (quoteHub is not null)
        {
            quoteHub.EndTransmission();
            quoteHub.Cache.Clear();
        }
    }

    [Benchmark]
    public object TestSeries()
    {
        return ExecuteIndicator(quotes, seriesListing);
    }

    [Benchmark]
    public object TestStream()
    {
        return ExecuteStreamIndicator(quoteHub, streamListing);
    }

    [Benchmark]
    public object TestBuffer()
    {
        return ExecuteBufferIndicator(quotes, bufferListing);
    }

    /// <summary>
    /// Executes a Series-style indicator using reflection.
    /// </summary>
    private static object ExecuteIndicator(IReadOnlyList<Quote> quoteData, IndicatorListing listing)
    {
        string methodName = listing.MethodName
            ?? throw new InvalidOperationException("MethodName is required");

        // Find the extension method
        MethodInfo method = FindExtensionMethod(methodName, typeof(IReadOnlyList<Quote>))
            ?? throw new InvalidOperationException($"Method {methodName} not found");

        // Build parameter array
        List<object> parameters = [quoteData];
        AddDefaultParameters(parameters, listing);

        // Invoke the method
        return method.Invoke(null, parameters.ToArray());
    }

    /// <summary>
    /// Executes a Stream-style indicator using reflection.
    /// </summary>
    private static object ExecuteStreamIndicator(QuoteHub hub, IndicatorListing listing)
    {
        string methodName = listing.MethodName
            ?? throw new InvalidOperationException("MethodName is required");

        // Find the extension method
        MethodInfo method = FindExtensionMethod(methodName, typeof(IQuoteProvider<IQuote>))
            ?? throw new InvalidOperationException($"Method {methodName} not found");

        // Build parameter array
        List<object> parameters = [hub];
        AddDefaultParameters(parameters, listing);

        // Invoke the method and get Results property
        object result = method.Invoke(null, parameters.ToArray())
            ?? throw new InvalidOperationException($"Method {methodName} returned null.");

        // Get Results property
        PropertyInfo resultsProperty = result.GetType().GetProperty("Results")
            ?? throw new InvalidOperationException($"Method {methodName} does not return a hub with Results.");

        return resultsProperty.GetValue(result);
    }

    /// <summary>
    /// Executes a Buffer-style indicator using reflection.
    /// </summary>
    private static object ExecuteBufferIndicator(IReadOnlyList<Quote> quoteData, IndicatorListing listing)
    {
        string methodName = listing.MethodName
            ?? throw new InvalidOperationException("MethodName is required");

        // Find the extension method
        MethodInfo method = FindExtensionMethod(methodName, typeof(IReadOnlyList<Quote>))
            ?? throw new InvalidOperationException($"Method {methodName} not found");

        // Build parameter array
        List<object> parameters = [quoteData];
        AddDefaultParameters(parameters, listing);

        // Invoke the method
        return method.Invoke(null, parameters.ToArray());
    }

    /// <summary>
    /// Finds an extension method by name and first parameter type.
    /// </summary>
    private static MethodInfo FindExtensionMethod(string methodName, Type firstParameterType)
    {
        Assembly assembly = typeof(Ema).Assembly;

        return assembly.GetTypes()
            .Where(t => t.IsClass && t.IsAbstract && t.IsSealed) // static classes
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .FirstOrDefault(m => {
                if (m.Name != methodName)
                {
                    return false;
                }

                ParameterInfo[] parameters = m.GetParameters();
                if (parameters.Length == 0)
                {
                    return false;
                }

                // Check if first parameter is assignable from the target type
                return parameters[0].ParameterType.IsAssignableFrom(firstParameterType)
                    || (parameters[0].ParameterType.IsGenericType
                        && firstParameterType.GetInterfaces().Any(
                            i => i.IsGenericType
                                && i.GetGenericTypeDefinition() == parameters[0].ParameterType.GetGenericTypeDefinition()));
            });
    }

    /// <summary>
    /// Adds default parameter values from the listing to the parameter array.
    /// </summary>
    private static void AddDefaultParameters(List<object> parameters, IndicatorListing listing)
    {
        if (listing.Parameters is null)
        {
            return;
        }

        foreach (IndicatorParam param in listing.Parameters)
        {
            if (param.DefaultValue is not null)
            {
                parameters.Add(param.DefaultValue);
            }
        }
    }
}
