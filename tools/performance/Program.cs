using BenchmarkDotNet.Running;

[assembly: CLSCompliant(false)]

namespace Performance;

public static class Program
{
    public static void Main(string[] args)
    {
        PerformanceConfig config = new();

        if (args?.Length == 0)
        {
            // run all with custom config
            // example: dotnet run -c Release
            BenchmarkRunner.Run(typeof(Program).Assembly, config);
        }
        else
        {
            // run based on arguments (e.g. filter)
            // example: dotnet run -c Release --filter *.GetAdx
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }

    /* USAGE
     * 
     * dotnet build -c Release
     * 
     * Examples, to run cohorts:
     * dotnet run -c Release --filter *Stream*
     * dotnet run -c Release --filter *External.Ema*
     * 
     * Performance results are exported to:
     * - BenchmarkDotNet.Artifacts/results/*.md (GitHub markdown)
     * - BenchmarkDotNet.Artifacts/results/*.json (JSON for analysis)
     */
}
