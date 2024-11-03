namespace Tests.Performance;

// INTERNAL FUNCTIONS

[ShortRunJob]
public class InternalsPerformance
{
    [Params(20, 50, 250, 1000)]
    public int Periods;

    private double[] values;

    // standard deviation
    [GlobalSetup(Targets = [nameof(StdDev)])]
    public void Setup()
        => values = TestData.GetLongish(Periods)
            .Select(x => (double)x.Close)
            .ToArray();

    [Benchmark]
    public object StdDev() => values.StdDev();
}
