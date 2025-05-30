using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Generators.Catalogger;
using System.Reflection;
using FluentAssertions;

namespace Tests.Generators;

[TestClass]
public class ResultTypeDetectionTests
{
    [TestMethod]
    public void SignalIndicatorShouldDetectSignalResultType()
    {
        // Arrange
        string source = @"
using Skender.Stock.Indicators;

namespace TestNamespace
{
    public static class SignalIndicator
    {
        [SeriesIndicator(""SI01"")]
        public static IReadOnlyList<SignalResult> ToSignal<TQuote>(
            this IReadOnlyList<TQuote> quotes,
            int period = 14)
            where TQuote : IQuote
        {
            return new List<SignalResult>();
        }

        public class SignalResult
        {
            public DateTime Date { get; set; }
            public decimal Value { get; set; }
            public bool Buy { get; set; }
            public bool Sell { get; set; }
        }
    }
}";

        // Act & Assert
        var generatedOutput = CompileAndValidate(source);

        // Check that the Signal result type was detected
        generatedOutput.Should().Contain("dataType: ResultType.Point");
    }

    [TestMethod]
    public void OscillatorIndicatorShouldDetectOscillatorResultType()
    {
        // Arrange
        string source = @"
using Skender.Stock.Indicators;

namespace TestNamespace
{
    public static class OscillatorIndicator
    {
        [SeriesIndicator(""OS01"")]
        public static IReadOnlyList<OscillatorResult> GetOscillator<TQuote>(
            this IReadOnlyList<TQuote> quotes)
            where TQuote : IQuote
        {
            return new List<OscillatorResult>();
        }

        public class OscillatorResult
        {
            public DateTime Date { get; set; }
            public decimal? Oscillator { get; set; }
        }
    }
}";

        // Act & Assert
        var generatedOutput = CompileAndValidate(source);

        // Check that the oscillator result type was detected
        generatedOutput.Should().Contain("dataType: ResultType.Centerline");
    }

    [TestMethod]
    public void BandIndicatorShouldDetectBandResultType()
    {
        // Arrange
        string source = @"
using Skender.Stock.Indicators;

namespace TestNamespace
{
    public static class BandIndicator
    {
        [SeriesIndicator(""BI01"")]
        public static IReadOnlyList<BandResult> ToBands<TQuote>(
            this IReadOnlyList<TQuote> quotes)
            where TQuote : IQuote
        {
            return new List<BandResult>();
        }

        public class BandResult
        {
            public DateTime Date { get; set; }
            public decimal? UpperBand { get; set; }
            public decimal? LowerBand { get; set; }
            public decimal? Centerline { get; set; }
        }
    }
}";

        // Act & Assert
        var generatedOutput = CompileAndValidate(source);

        // Check that the band result type was detected
        generatedOutput.Should().Contain("dataType: ResultType.Channel");
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
        driver = driver.RunGenerators(compilation);

        // Get the generated output
        var result = driver.GetRunResult().Results.First();
        var generatedCode = result.GeneratedSources.First().SourceText.ToString();

        return generatedCode;
    }
}
