using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Loggers;

namespace Performance;

/// <summary>
/// BenchmarkDotNet configuration for performance testing.
/// </summary>
public class PerformanceConfig : ManualConfig
{
    public PerformanceConfig()
    {
        // Add standard exporters for different formats
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(JsonExporter.Full);

        // Add columns for detailed analysis
        AddColumn(TargetMethodColumn.Method);
        AddColumn(StatisticColumn.Mean);
        AddColumn(StatisticColumn.Error);
        AddColumn(StatisticColumn.StdDev);

        // Add logger
        AddLogger(ConsoleLogger.Default);
    }
}
