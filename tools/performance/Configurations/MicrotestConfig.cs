using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
namespace Performance;

/// <summary>
/// Job config tuned for micro-benchmarks (very-fast operations).
/// Ensures each measured iteration runs long enough to be reliable.
/// </summary>
public sealed class MicrotestConfig : ManualConfig
{
    public MicrotestConfig()
    {
        Job job = Job.Default
            .WithEvaluateOverhead(true)
            .WithMinInvokeCount(25);

        AddJob(job);
    }
}
