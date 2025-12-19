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

    private static readonly IReadOnlyList<Quote> quotes = Data.GetRandom(Periods);
    private static IndicatorListing seriesListing;
    private static IndicatorListing streamListing;
    private static IndicatorListing bufferListing;
    private readonly QuoteHub quoteHub = new();

    [GlobalSetup]
    public void Setup()
    {
        // Find indicator listings by keyword
        IReadOnlyCollection<IndicatorListing> matches = Catalog.Search(Keyword);

        if (matches.Count == 0)
        {
            throw new InvalidOperationException($"No indicators found matching keyword: {Keyword}");
        }

        // Get listings for each style
        seriesListing = matches.FirstOrDefault(x => x.Style == Style.Series);
        streamListing = matches.FirstOrDefault(x => x.Style == Style.Stream);
        bufferListing = matches.FirstOrDefault(x => x.Style == Style.Buffer);

        // Populate quote hub for stream tests (not counted in benchmark time)
        if (streamListing != null)
        {
            quoteHub.Add(quotes);
        }

        // Log configuration
        Console.WriteLine("Manual Performance Test Configuration:");
        Console.WriteLine($"  Keyword: {Keyword}");
        Console.WriteLine($"  Periods: {Periods:N0}");
        Console.WriteLine($"  Series: {seriesListing?.MethodName ?? "N/A"}");
        Console.WriteLine($"  Stream: {streamListing?.MethodName ?? "N/A"}");
        Console.WriteLine($"  Buffer: {bufferListing?.MethodName ?? "N/A"}");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (streamListing != null)
        {
            quoteHub.EndTransmission();
            quoteHub.Cache.Clear();
        }
    }

    [Benchmark]
    public object TestSeries()
    {
        if (seriesListing == null)
        {
            return null;
        }

        return ExecuteIndicator(quotes, seriesListing);
    }

    [Benchmark]
    public object TestStream()
    {
        if (streamListing == null)
        {
            return null;
        }

        return ExecuteStreamIndicator(quoteHub, streamListing);
    }

    [Benchmark]
    public object TestBuffer()
    {
        if (bufferListing == null)
        {
            return null;
        }

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
        MethodInfo method = FindExtensionMethod(methodName, typeof(IReadOnlyList<Quote>));

        if (method == null)
        {
            throw new InvalidOperationException($"Method {methodName} not found");
        }

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
        MethodInfo method = FindExtensionMethod(methodName, typeof(IQuoteProvider<IQuote>));

        if (method == null)
        {
            throw new InvalidOperationException($"Method {methodName} not found");
        }

        // Build parameter array
        List<object> parameters = [hub];
        AddDefaultParameters(parameters, listing);

        // Invoke the method and get Results property
        object result = method.Invoke(null, parameters.ToArray());

        if (result == null)
        {
            return null;
        }

        // Get Results property
        PropertyInfo resultsProperty = result.GetType().GetProperty("Results");
        return resultsProperty?.GetValue(result);
    }

    /// <summary>
    /// Executes a Buffer-style indicator using reflection.
    /// </summary>
    private static object ExecuteBufferIndicator(IReadOnlyList<Quote> quoteData, IndicatorListing listing)
    {
        string methodName = listing.MethodName
            ?? throw new InvalidOperationException("MethodName is required");

        // Find the extension method
        MethodInfo method = FindExtensionMethod(methodName, typeof(IReadOnlyList<Quote>));

        if (method == null)
        {
            throw new InvalidOperationException($"Method {methodName} not found");
        }

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
        if (listing.Parameters == null)
        {
            return;
        }

        foreach (IndicatorParam param in listing.Parameters)
        {
            if (param.DefaultValue != null)
            {
                parameters.Add(param.DefaultValue);
            }
        }
    }
}
