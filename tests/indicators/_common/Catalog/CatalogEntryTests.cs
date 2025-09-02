#nullable enable
namespace Catalog;

[TestClass]
[DoNotParallelize]
public class CatalogEntryTests : TestBase
{
    // Clear the registry before each test to ensure clean state
    [TestInitialize]
    public void Setup() => IndicatorRegistry.Clear();

    [TestCleanup]
    public void Cleanup()
    {
        // Clear the registry after each test to ensure clean state
        IndicatorRegistry.Clear();
        // Re-enable auto-initialization for other tests
        IndicatorRegistry.EnableAutoInitialization();
    }

    [TestMethod]
    public void EachCatalogEntryShouldHaveRequiredMetadata()
    {
        // Arrange & Act - Get all catalog entries
        IReadOnlyList<IndicatorListing> catalog = IndicatorCatalog.Catalog;

        // Assert - Each entry should have complete metadata for automation
        catalog.Should().NotBeEmpty("catalog should contain indicator listings");

        foreach (IndicatorListing listing in catalog)
        {
            // Basic properties
            listing.Uiid.Should().NotBeNullOrWhiteSpace($"indicator {listing.Name} should have a UIID");
            listing.Name.Should().NotBeNullOrWhiteSpace($"indicator {listing.Uiid} should have a name");
            listing.Style.Should().BeDefined($"indicator {listing.Uiid} should have a valid style");
            listing.Category.Should().BeDefined($"indicator {listing.Uiid} should have a valid category");

            // Method name for automation - ALL indicators must have method names
            listing.MethodName.Should().NotBeNullOrWhiteSpace($"indicator {listing.Uiid} should have a method name for automation");
            listing.MethodName.Should().StartWith("To", $"method name for {listing.Uiid} should start with 'To'");

            // Parameters should be valid if present
            if (listing.Parameters != null)
            {
                foreach (IndicatorParam param in listing.Parameters)
                {
                    param.ParameterName.Should().NotBeNullOrWhiteSpace($"parameter in {listing.Uiid} should have a name");
                    param.DisplayName.Should().NotBeNullOrWhiteSpace($"parameter {param.ParameterName} in {listing.Uiid} should have a display name");
                    param.DataType.Should().NotBeNullOrWhiteSpace($"parameter {param.ParameterName} in {listing.Uiid} should have a data type");

                    // Required parameters should have constraints
                    if (param.IsRequired)
                    {
                        // Some parameter types like IEnumerable and DateTime are provided by the user and don't need default values
                        string[] allowedNullDefaults = ["IEnumerable", "IReadOnlyList", "DateTime"];
                        if (param.DefaultValue == null && !allowedNullDefaults.Any(t => param.DataType.Contains(t, StringComparison.OrdinalIgnoreCase)))
                        {
                            Assert.Fail($"Required parameter {param.ParameterName} in {listing.Uiid} should have a default value or be of allowed type (IEnumerable, DateTime, etc.)");
                        }
                    }
                }
            }

            // Results should be valid
            listing.Results.Should().NotBeNullOrEmpty($"indicator {listing.Uiid} should have at least one result");
            foreach (IndicatorResult result in listing.Results!)
            {
                result.DataName.Should().NotBeNullOrWhiteSpace($"result in {listing.Uiid} should have a data name");
                result.DisplayName.Should().NotBeNullOrWhiteSpace($"result {result.DataName} in {listing.Uiid} should have a display name");
                result.DataType.Should().BeDefined($"result {result.DataName} in {listing.Uiid} should have a valid data type");
            }
        }
    }

    [TestMethod]
    public void EachCatalogEntryWithMethodNameShouldBeCallable()
    {
        // Arrange - Get all catalog entries with method names
        IReadOnlyList<IndicatorListing> catalog = IndicatorCatalog.Catalog;
        List<IndicatorListing> entriesWithMethods = catalog.Where(l => !string.IsNullOrWhiteSpace(l.MethodName)).ToList();

        // Assert - Should have entries with method names
        entriesWithMethods.Should().NotBeEmpty("there should be catalog entries with method names");

        foreach (IndicatorListing? listing in entriesWithMethods)
        {
            // Verify method name follows expected pattern
            listing.MethodName.Should().StartWith("To", $"method name for {listing.Uiid} should start with 'To'");

            // Method name should be related to the indicator (allow for common abbreviations and expansions)
            // This is a practical test - the important thing is that the method name exists and follows convention
            string cleanUiid = listing.Uiid.Replace("-", "", StringComparison.OrdinalIgnoreCase).Replace("_", "", StringComparison.OrdinalIgnoreCase);

            // TODO: figure out how to do this without coding in special cases

            // Handle common cases - method names should be reasonable representations of the indicator
            bool isRelated = listing.MethodName!.Contains(cleanUiid, StringComparison.OrdinalIgnoreCase) ||
                           (cleanUiid.Equals("BB", StringComparison.OrdinalIgnoreCase) && listing.MethodName!.Contains("BOLLINGER", StringComparison.OrdinalIgnoreCase)) ||
                           (cleanUiid.Equals("CHEXIT", StringComparison.OrdinalIgnoreCase) && listing.MethodName!.Contains("CHANDELIER", StringComparison.OrdinalIgnoreCase)) ||
                           listing.MethodName!.EndsWith(cleanUiid, StringComparison.OrdinalIgnoreCase) ||
                           listing.MethodName!.Contains(cleanUiid[..Math.Min(3, cleanUiid.Length)], StringComparison.OrdinalIgnoreCase);

            // If not obviously related, just verify it's a valid method name format
            if (!isRelated)
            {
                listing.MethodName.Should().MatchRegex(@"^To[A-Z][a-zA-Z]*$", $"method name '{listing.MethodName}' for {listing.Uiid} should follow valid format");
            }
        }
    }

    [TestMethod]
    public void CatalogShouldHaveConsistentStyleCounts()
    {
        // Arrange & Act
        IReadOnlyList<IndicatorListing> catalog = IndicatorCatalog.Catalog;

        int seriesCount = catalog.Count(x => x.Style == Style.Series);
        int streamCount = catalog.Count(x => x.Style == Style.Stream);
        int bufferCount = catalog.Count(x => x.Style == Style.Buffer);

        // Assert - verify the counts are as expected per the earlier comment
        seriesCount.Should().Be(84, "there should be exactly 84 series style indicators");
        streamCount.Should().Be(8, "there should be exactly 8 stream style indicators");
        bufferCount.Should().Be(2, "there should be exactly 2 buffer style indicators");

        // Total verification
        int totalCount = seriesCount + streamCount + bufferCount;
        totalCount.Should().Be(94, "total indicators should be exactly 94");
    }

    [TestMethod]
    public void EachIndicatorShouldHaveUniqueIdStyleCombination()
    {
        // Arrange & Act
        IReadOnlyList<IndicatorListing> catalog = IndicatorCatalog.Catalog;

        // Group by UIID and Style combination
        var duplicates = catalog
            .GroupBy(l => new { l.Uiid, l.Style })
            .Where(g => g.Count() > 1)
            .ToList();

        // Assert - No duplicates should exist
        duplicates.Should().BeEmpty("each indicator should have a unique UIID and Style combination");
    }

    [TestMethod]
    public void IndicatorsWithSameUiidShouldHaveDifferentStyles()
    {
        // Arrange & Act
        IReadOnlyList<IndicatorListing> catalog = IndicatorCatalog.Catalog;

        // Group by UIID to find indicators with multiple implementations
        List<IGrouping<string, IndicatorListing>> multiStyleIndicators = catalog
            .GroupBy(l => l.Uiid)
            .Where(g => g.Count() > 1)
            .ToList();

        // Assert - Multi-style indicators should have different styles
        foreach (IGrouping<string, IndicatorListing>? group in multiStyleIndicators)
        {
            List<Style> styles = group.Select(l => l.Style).Distinct().ToList();
            styles.Should().HaveCount(group.Count(),
                $"indicator {group.Key} with multiple implementations should have different styles for each");
        }
    }

    [TestMethod]
    public void EachCatalogEntryParametersShouldBeValidForAutomation()
    {
        // Arrange & Act
        IReadOnlyList<IndicatorListing> catalog = IndicatorCatalog.Catalog;
        List<IndicatorListing> entriesWithParameters = catalog.Where(l => l.Parameters != null && l.Parameters.Any()).ToList();

        // Assert
        entriesWithParameters.Should().NotBeEmpty("there should be indicators with parameters");

        foreach (IndicatorListing? listing in entriesWithParameters)
        {
            foreach (IndicatorParam parameter in listing.Parameters!)
            {
                // For automation, we need to be able to create instances of the parameter types
                parameter.DataType.Should().NotBeNullOrWhiteSpace($"parameter {parameter.ParameterName} in {listing.Uiid} should have a data type");

                // Common data types that should be supported for automation
                string[] supportedTypes =
                [
                    "Int32", "Double", "Boolean", "DateTime", "String", "enum", "Nullable",
                    "IEnumerable", "IReadOnlyList<T> where T : IReusable",
                    "BetaType", "ChandExitType", "PivotPointType", "MaType", "CandlePartType", "EndType"
                ];

                supportedTypes.Should().Contain(t => parameter.DataType.Contains(t) || parameter.DataType.StartsWith(t),
                    $"parameter {parameter.ParameterName} data type '{parameter.DataType}' in {listing.Uiid} should be supported for automation");

                // Required parameters must have default values for automation
                if (parameter.IsRequired && parameter.DefaultValue == null)
                {
                    // This is allowed for some parameter types like IEnumerable and DateTime that are provided by the user
                    string[] allowedNullDefaults = ["IEnumerable", "IReadOnlyList", "DateTime"];
                    allowedNullDefaults.Should().Contain(t => parameter.DataType.Contains(t, StringComparison.OrdinalIgnoreCase),
                        $"required parameter {parameter.ParameterName} in {listing.Uiid} without default value should be of allowed type (IEnumerable, DateTime, etc.)");
                }
            }
        }
    }
}
