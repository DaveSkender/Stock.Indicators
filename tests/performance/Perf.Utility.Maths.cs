namespace Performance;

// INTERNAL UTILITIES

[ShortRunJob]
public class UtilityMaths
{
    [Params(20, 50, 250, 1000)]
    public int Periods;

    private double[] _values;

    // standard deviation
    [GlobalSetup(Targets = [nameof(StdDev)])]
    public void Setup()
        => _values = Data.GetLongish(Periods)
            .Select(x => (double)x.Close)
            .ToArray();

    [Benchmark]
    public object StdDev() => _values.StdDev();
}
