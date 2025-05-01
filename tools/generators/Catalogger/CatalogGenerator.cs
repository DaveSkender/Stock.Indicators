namespace Generators.Catalogger;

/// <summary>
/// Source generator for catalog information about indicators from attributes.
/// </summary>
[Generator]
public class CatalogGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the syntax providers for different attribute types
        IncrementalValuesProvider<SyntaxNode> seriesProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForAttribute(s, "Series", "SeriesAttribute"),
                transform: static (ctx, _) => ctx.Node)
            .Where(static m => m is not null)!;

        // Stream attributes on methods
        IncrementalValuesProvider<SyntaxNode> streamProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForAttribute(s, "StreamHub", "StreamAttribute"),
                transform: static (ctx, _) => ctx.Node)
            .Where(static m => m is not null)!;

        // Buffer attributes on constructors
        IncrementalValuesProvider<SyntaxNode> bufferProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForAttribute(s, "Buffer", "BufferAttribute"),
                transform: static (ctx, _) => ctx.Node)
            .Where(static m => m is not null)!;

        // Combine all providers with compilation
        IncrementalValueProvider<ImmutableArray<SyntaxNode>> compiledSeriesProvider = seriesProvider.Collect();
        IncrementalValueProvider<ImmutableArray<SyntaxNode>> compiledStreamProvider = streamProvider.Collect();
        IncrementalValueProvider<ImmutableArray<SyntaxNode>> compiledBufferProvider = bufferProvider.Collect();

        IncrementalValueProvider<(((
            Compilation Left,
            ImmutableArray<SyntaxNode> Right) Left,
            ImmutableArray<SyntaxNode> Right) Left,
            ImmutableArray<SyntaxNode> Right)> combinedProvider
            = context.CompilationProvider
             .Combine(compiledSeriesProvider)
             .Combine(compiledStreamProvider)
             .Combine(compiledBufferProvider);

        // Register the source output generation
        context.RegisterSourceOutput(
            combinedProvider,
            (spc, tuple) => Execute(
                spc,
                tuple.Left.Left.Left,
                tuple.Left.Left.Right,
                tuple.Left.Right,
                tuple.Right));
    }

    private static bool IsCandidateForAttribute(
        SyntaxNode node, params string[] attributeNames)
        => node is MemberDeclarationSyntax member
            && member.AttributeLists.Count != 0
            && member.AttributeLists.Any(
                al => al.Attributes.Any(
                    a => attributeNames.Any(
                        attr => a.Name.ToString().Contains(attr))));

    private static void Execute(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<SyntaxNode> seriesNodes,
        ImmutableArray<SyntaxNode> streamNodes,
        ImmutableArray<SyntaxNode> bufferNodes)
    {
        // Get required symbol references
        Dictionary<string, INamedTypeSymbol?> requiredSymbols = GetRequiredSymbols(compilation);

        if (!AreRequiredSymbolsAvailable(requiredSymbols))
        {
            return;
        }

        List<IndicatorInfo> indicators = [];
        HashSet<string> processedIds = [];

        // Process nodes for each attribute type
        IndicatorProcessor.ProcessNodes(context, compilation, "Series", seriesNodes, requiredSymbols, indicators, processedIds);
        IndicatorProcessor.ProcessNodes(context, compilation, "Stream", streamNodes, requiredSymbols, indicators, processedIds);
        IndicatorProcessor.ProcessNodes(context, compilation, "Buffer", bufferNodes, requiredSymbols, indicators, processedIds);

        // Validate indicator UIIDs are unique before generating code
        Validate(context, indicators);

        // Generate and output the catalog code
        CodeGenerator.GenerateOutput(context, indicators);
    }

    private static void Validate(SourceProductionContext context, List<IndicatorInfo> indicators)
    {
        List<string> duplicateUIIDs = indicators
            .GroupBy(x => x.Uiid)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateUIIDs.Count != 0)
        {
            context.ReportDiagnostic(
                diagnostic: Diagnostic.Create(
                    descriptor: DiagnosticDescriptors.IND901_DuplicateUiidFoundDescriptor,
                    location: Location.None,
                    messageArgs: string.Join(", ", duplicateUIIDs)));
        }
    }

    private static Dictionary<string, INamedTypeSymbol?> GetRequiredSymbols(Compilation compilation)
    {
        const string srcNamespace = "Skender.Stock.Indicators";

        return new() {
            ["SeriesAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.SeriesAttribute"),
            ["StreamAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.StreamAttribute"),
            ["BufferAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.BufferAttribute"),
            ["ParamAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.ParamAttribute`1"),
            ["Category"] = compilation.GetTypeByMetadataName($"{srcNamespace}.Category"),
            ["ChartType"] = compilation.GetTypeByMetadataName($"{srcNamespace}.ChartType")
        };
    }

    private static bool AreRequiredSymbolsAvailable(
        Dictionary<string, INamedTypeSymbol?> symbols)
        => symbols.Values.All(symbol => symbol != null);
}
