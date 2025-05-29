using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Generators.Catalogger;
using System.Reflection;
using FluentAssertions;

namespace Tests.Generators;

[TestClass]
public class DocumentationParsingTests
{
    [TestMethod]
    public void XmlDocumentationShouldBeParsed()
    {
        // Arrange
        string source = @"
using Skender.Stock.Indicators;

namespace TestNamespace
{
    public static class DocumentedIndicator
    {
        /// <summary>
        /// Calculates the documented indicator.
        /// This documentation should be extracted.
        /// </summary>
        /// <param name=""quotes"">Historical price quotes.</param>
        /// <param name=""lookbackPeriods"">Number of periods for lookback.</param>
        /// <returns>Collection of indicator results.</returns>
        [SeriesIndicator(""DOC01"")]
        public static IReadOnlyList<DocResult> ToDocumentedIndicator<TQuote>(
            this IReadOnlyList<TQuote> quotes,
            int lookbackPeriods = 14)
            where TQuote : IQuote
        {
            return new List<DocResult>();
        }

        /// <summary>
        /// Result for the documented indicator.
        /// </summary>
        public class DocResult
        {
            /// <summary>
            /// Date of the observation.
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// Main indicator value.
            /// </summary>
            public decimal Value { get; set; }
        }
    }
}";

        // Act & Assert
        var generatedOutput = CompileAndValidate(source);

        // Check that XML documentation was parsed and used
        generatedOutput.Should().Contain("description: \"Calculates the documented indicator");
        generatedOutput.Should().Contain("description: \"Number of periods for lookback\"");
    }

    private static string CompileAndValidate(string source)
    {
        // Create compilation with the source code and documentation
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
        driver = driver.RunGenerators(compilation);

        // Get the generated output
        var result = driver.GetRunResult().Results.First();
        var generatedCode = result.GeneratedSources.First().SourceText.ToString();

        return generatedCode;
    }
}
