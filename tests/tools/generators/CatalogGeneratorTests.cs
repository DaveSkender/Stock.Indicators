using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Generators.Catalogger;
using System.Reflection;
using FluentAssertions;

namespace Tests.Generators;

[TestClass]
public class CatalogGeneratorTests
{
    [TestMethod]
    public void SeriesIndicatorShouldGenerateProperListing()
    {
        // Arrange
        string source = @"
using Skender.Stock.Indicators;

namespace TestNamespace
{
    public static class TestIndicator
    {
        [SeriesIndicator(""TI01"")]
        public static IReadOnlyList<TestResult> ToTestIndicator<TQuote>(
            this IReadOnlyList<TQuote> quotes,
            int lookbackPeriods = 14)
            where TQuote : IQuote
        {
            return new List<TestResult>();
        }

        public class TestResult
        {
            public DateTime Date { get; set; }
            public decimal Value { get; set; }
        }
    }
}";

        // Act & Assert
        var generatedOutput = CompileAndValidate(source);

        // Check that the listing was generated
        generatedOutput.Should().Contain("public static IndicatorListing SeriesListing { get; }");

        // Check that parameters and results are extracted
        generatedOutput.Should().Contain("parameterName: \"lookbackPeriods\"");
        generatedOutput.Should().Contain("dataType: ResultType.Default");
    }

    [TestMethod]
    public void PartialClassShouldGeneratePartialImplementation()
    {
        // Arrange
        string source = @"
using Skender.Stock.Indicators;

namespace TestNamespace
{
    public static partial class TestIndicator
    {
        [SeriesIndicator(""TI02"")]
        public static IReadOnlyList<TestResult> ToTestIndicator<TQuote>(
            this IReadOnlyList<TQuote> quotes,
            int period = 20,
            decimal? signal = 0.5m)
            where TQuote : IQuote
        {
            return new List<TestResult>();
        }

        public class TestResult
        {
            public DateTime Date { get; set; }
            public decimal Value { get; set; }
        }
    }
}";

        // Act & Assert
        var generatedOutput = CompileAndValidate(source);

        // Check that the partial keyword is present
        generatedOutput.Should().Contain("public partial class TestIndicator");

        // Check that both parameters are extracted
        generatedOutput.Should().Contain("parameterName: \"period\"");
        generatedOutput.Should().Contain("parameterName: \"signal\"");
    }

    [TestMethod]
    public void ClassWithExistingListingShouldNotGenerateNewOne()
    {
        // Arrange
        string source = @"
using Skender.Stock.Indicators;

namespace TestNamespace
{
    public static class TestIndicator
    {
        [SeriesIndicator(""TI03"")]
        public static IReadOnlyList<TestResult> ToTestIndicator<TQuote>(
            this IReadOnlyList<TQuote> quotes)
            where TQuote : IQuote
        {
            return new List<TestResult>();
        }

        public class TestResult
        {
            public DateTime Date { get; set; }
            public decimal Value { get; set; }
        }

        public static IndicatorListing Listing =>
            new IndicatorListingBuilder()
                .WithName(""Test"")
                .WithId(""TEST"")
                .WithStyle(Style.Series)
                .Build();
    }
}";

        // Act & Assert
        // The generator should not produce any output for classes that already have listings
        try
        {
            var generatedOutput = CompileAndValidate(source);
            // If we get here, something was generated when it shouldn't have been
            Assert.Fail("Expected no source generation for class with existing listing");
        }
        catch (InvalidOperationException ex) when (ex.Message == "No source files generated")
        {
            // This is the expected behavior - no source files should be generated
            // when a class already has a listing
        }
    }

    private static string CompileAndValidate(string source)
    {
        // Create compilation with the source code
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create references to necessary assemblies
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
            MetadataReference.CreateFromFile(typeof(SeriesIndicatorAttribute).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Create an instance of the generator
        var generator = new CatalogGenerator();

        // Run the generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        var runResult = driver.RunGenerators(compilation);

        // Get the generated output
        var result = runResult.GetRunResult();

        // Check for diagnostics/errors
        if (result.Diagnostics.Any())
        {
            var diagnostics = string.Join("\n", result.Diagnostics);
            throw new InvalidOperationException($"Generator diagnostics: {diagnostics}");
        }

        if (!result.Results.Any())
        {
            throw new InvalidOperationException("No generator results produced");
        }

        var generatorResult = result.Results.First();

        if (!generatorResult.GeneratedSources.Any())
        {
            throw new InvalidOperationException("No source files generated");
        }

        var generatedCode = generatorResult.GeneratedSources.First().SourceText.ToString();

        return generatedCode;
    }
}
