using BenchmarkDotNet.Attributes;
using Tests.Indicators.Baselines;

namespace Performance;

// REGRESSION TEST PERFORMANCE BENCHMARKS (T024)

[ShortRunJob]
public class RegressionTests
{
    private static readonly IReadOnlyList<Quote> q = Data.GetDefault();

    [Benchmark]
    public void BaselineComparerIdenticalResults()
    {
        // Benchmark exact binary equality comparison
        List<SmaResult> expected = q.ToSma(20).ToList();
        List<SmaResult> actual = q.ToSma(20).ToList();

        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        if (!result.IsMatch)
        {
            throw new InvalidOperationException("Comparison failed");
        }
    }

    [Benchmark]
    public void BaselineComparerLargeDataset()
    {
        // Benchmark with larger dataset (10,000 items)
        List<SmaResult> expected = Enumerable.Range(0, 10000)
            .Select(i => new SmaResult(DateTime.Today.AddDays(i), i * 1.5))
            .ToList();

        List<SmaResult> actual = expected.ToList();

        ComparisonResult result = BaselineComparer.Compare(expected, actual);

        if (!result.IsMatch)
        {
            throw new InvalidOperationException("Comparison failed");
        }
    }

    [Benchmark]
    public void RegressionTestSMA()
    {
        // Benchmark full regression test execution for SMA
        IReadOnlyList<SmaResult> results = q.ToSma(20);

        // Simulate baseline comparison (without file I/O)
        List<SmaResult> baseline = results.ToList();
        ComparisonResult comparison = BaselineComparer.Compare(baseline, results);

        if (!comparison.IsMatch)
        {
            throw new InvalidOperationException("Regression test failed");
        }
    }

    [Benchmark]
    public void RegressionTestMACD()
    {
        // Benchmark multi-property indicator regression test
        IReadOnlyList<MacdResult> results = q.ToMacd(12, 26, 9);

        List<MacdResult> baseline = results.ToList();
        ComparisonResult comparison = BaselineComparer.Compare(baseline, results);

        if (!comparison.IsMatch)
        {
            throw new InvalidOperationException("Regression test failed");
        }
    }

    [Benchmark]
    public void RegressionTestRSI()
    {
        // Benchmark complex calculation indicator
        IReadOnlyList<RsiResult> results = q.ToRsi(14);

        List<RsiResult> baseline = results.ToList();
        ComparisonResult comparison = BaselineComparer.Compare(baseline, results);

        if (!comparison.IsMatch)
        {
            throw new InvalidOperationException("Regression test failed");
        }
    }
}
