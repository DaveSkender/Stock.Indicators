namespace Tests.Performance;

// INTERNAL UTILITIES

[ShortRunJob]
public class UtilityStdDev
{
    [Params(20, 50, 250, 1000)]
    public int Periods;

    private double[] _values;

    // standard deviation
    [GlobalSetup(Targets = [nameof(StdDev)])]
    public void Setup()
        => _values = TestData.GetLongish(Periods)
            .Select(x => (double)x.Close)
            .ToArray();

    [Benchmark]
    public object StdDev() => _values.StdDev();
}
