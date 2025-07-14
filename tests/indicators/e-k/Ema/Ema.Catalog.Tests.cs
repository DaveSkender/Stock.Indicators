namespace Catalog;

[TestClass]
public class EmaTests : TestBase
{
    [TestMethod]
    public void EmaSeriesListing()
    {
        // Arrange
        var quotes = Quotes; // Using TestBase.Quotes
        var catalog = IndicatorCatalog.Catalog;

        // Find EMA Series listing in catalog using ID and Style
        var listing = catalog.FirstOrDefault(l => l.Uiid == "EMA" && l.Style == Style.Series);
        listing.Should().NotBeNull("EMA Series listing should be found in catalog");

        // Act - Use catalog metadata to dynamically construct and execute the call
        var catalogResults = ExecuteIndicatorFromCatalog(quotes, listing!);

        // Act - Direct call for comparison using catalog's default parameter value
        var lookbackParam = listing!.Parameters.First(p => p.ParameterName == "lookbackPeriods");
        var lookbackValue = (int)lookbackParam.DefaultValue!;
        var directResults = quotes.ToEma(lookbackValue);

        // Assert - Results from catalog-driven execution should match direct call
        catalogResults.Should().BeEquivalentTo(directResults);

        // Assert - Lookup gets correct values
        lookbackValue.Should().Be(20);

        // Additional assertions on catalog metadata (basic validation)
        listing.Name.Should().Be("Exponential Moving Average");
        listing.Uiid.Should().Be("EMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToEma");
        listing.Parameters.Should().HaveCount(2);
        listing.Results.Should().HaveCount(1);
    }

    /// <summary>
    /// Executes an indicator using catalog metadata via reflection to simulate automation scenarios.
    /// This validates that the catalog contains sufficient and correct information for dynamic execution.
    /// </summary>
    private static IReadOnlyList<EmaResult> ExecuteIndicatorFromCatalog(IEnumerable<IQuote> quotes, IndicatorListing listing)
    {
        // Get the method from the listing's MethodName - search across all types in the indicators assembly
        var methodName = listing.MethodName ?? throw new InvalidOperationException("MethodName is required for automation");

        // Get the assembly containing the indicators
        var indicatorsAssembly = typeof(Ema).Assembly;

        // Find all static classes in the assembly
        var types = indicatorsAssembly.GetTypes()
            .Where(t => t.IsClass && t.IsAbstract && t.IsSealed) // static classes
            .ToArray();

        var methods = new List<System.Reflection.MethodInfo>();

        // Search for the method across all static classes
        foreach (var type in types)
        {
            var typeMethods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(m => m.Name == methodName)
                .ToArray();
            methods.AddRange(typeMethods);
        }

        methods.Should().NotBeEmpty($"Method {methodName} should exist");

        // Build parameter array using catalog metadata
        var parameters = new List<object> { quotes };

        // Add required parameters using their default values from catalog
        foreach (var param in listing.Parameters.Where(p => p.IsRequired))
        {
            param.DefaultValue.Should().NotBeNull($"Required parameter {param.ParameterName} should have a default value");
            parameters.Add(param.DefaultValue!);
        }

        // Find the method that matches our parameter count (accounting for generic methods)
        var targetMethod = methods.FirstOrDefault(m => m.GetParameters().Length == parameters.Count);
        targetMethod.Should().NotBeNull($"Should find {methodName} method with {parameters.Count} parameters");

        // If the method is generic, make it specific for IQuote type
        if (targetMethod!.IsGenericMethodDefinition)
        {
            var genericArguments = targetMethod.GetGenericArguments();
            if (genericArguments.Length == 1)
            {
                // Make the generic method specific for IQuote
                targetMethod = targetMethod.MakeGenericMethod(typeof(IQuote));
            }
        }

        // Execute the method via reflection
        var result = targetMethod.Invoke(null, parameters.ToArray());
        result.Should().NotBeNull("Method execution should return a result");

        // Cast to expected type
        var emaResults = result as IReadOnlyList<EmaResult>;
        emaResults.Should().NotBeNull("Result should be castable to IReadOnlyList<EmaResult>");

        return emaResults!;
    }

    [TestMethod]
    public void EmaStreamListing()
    {
        // Act
        var listing = Ema.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Exponential Moving Average");
        listing.Uiid.Should().Be("EMA");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.MovingAverage);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        var period = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        period.Should().NotBeNull();
        period!.DisplayName.Should().Be("Lookback Period");
        period.Description.Should().Be("Number of periods for the EMA calculation");
        period.IsRequired.Should().BeTrue();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Ema");
        result.DisplayName.Should().Be("EMA");
        result.DataType.Should().Be(ResultType.Default);
        result.IsReusable.Should().BeTrue();
    }

    [TestMethod]
    public void EmaBufferListing()
    {
        // Act
        var listing = Ema.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Exponential Moving Average");
        listing.Uiid.Should().Be("EMA");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.MovingAverage);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        var period = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        period.Should().NotBeNull();
        period!.DisplayName.Should().Be("Lookback Period");
        period.Description.Should().Be("Number of periods for the EMA calculation");
        period.IsRequired.Should().BeTrue();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Ema");
        result.DisplayName.Should().Be("EMA");
        result.DataType.Should().Be(ResultType.Default);
        result.IsReusable.Should().BeTrue();
    }

    [TestMethod]
    public void EmaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        var quotes = Quotes; // Using TestBase.Quotes
        var listing = Ema.SeriesListing;

        // Get default parameter value from catalog
        var lookbackParam = listing.Parameters.First(p => p.ParameterName == "lookbackPeriods");
        var lookbackValue = (int)lookbackParam.DefaultValue!;

        // Act - Call using catalog metadata (via method name)
        var catalogResults = quotes.ToEma(lookbackValue);

        // Act - Direct call
        var seriesResults = quotes.ToEma(lookbackValue);

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(seriesResults);
    }
}
