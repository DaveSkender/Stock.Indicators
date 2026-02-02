using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;

namespace Performance;

/// <summary>
/// BenchmarkDotNet configuration for performance testing (default).
/// </summary>
public class DefaultConfig : ManualConfig
{
    public DefaultConfig()
    {
        // Add standard exporters for different formats
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(JsonExporter.Full);

        // Add columns for detailed analysis
        AddColumn(TargetMethodColumn.Method);
        AddColumn(StatisticColumn.Mean);
        AddColumn(StatisticColumn.Error);
        AddColumn(StatisticColumn.StdDev);

        // Add memory diagnostics with GC collection columns
        AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(displayGenColumns: true)));

        // Sort results by method name
        WithOrderer(new DefaultOrderer(SummaryOrderPolicy.Method));

        // Add logger
        AddLogger(ConsoleLogger.Default);
    }
}
