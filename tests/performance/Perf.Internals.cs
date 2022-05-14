using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance;

// INTERNAL FUNCTIONS

[MarkdownExporterAttribute.GitHub]
public class InternalsPerformance
{
    // standard deviation

    private double[] values;

    [Params(20, 50, 250, 1000)]
    public int Periods;

    [GlobalSetup(Targets = new[] { nameof(StdDev) })]
    public void Setup()
        => values = TestData.GetLongish(Periods)
            .Select(x => (double)x.Close)
            .ToArray();

    [Benchmark]
    public object StdDev() => Functions.StdDev(values);
}
