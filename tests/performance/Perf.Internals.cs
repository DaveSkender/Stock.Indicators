using BenchmarkDotNet.Attributes;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Performance;

// INTERNAL FUNCTIONS

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
    public object StdDev() => values.StdDev();
}
