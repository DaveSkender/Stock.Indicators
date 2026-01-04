namespace Performance;

// INTERNAL UTILITIES

[Config(typeof(MicrotestConfig))]
[WarmupCount(5), IterationCount(10), IterationTime(1000)]
public class UtilityStdDev
{
    private const int OpsQty = 8;

    [Params(20, 50, 250, 1000)]
    public int Periods;

    private double[] _values;

    // standard deviation
    [GlobalSetup(Targets = [nameof(StdDev)])]
    public void Setup()
        => _values = Data.GetLongish(Periods)
            .Select(static x => (double)x.Close)
            .ToArray();

    [Benchmark(OperationsPerInvoke = OpsQty)]
    public object StdDev() => _values.StdDev();
}
