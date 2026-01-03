using BenchmarkDotNet.Running;

namespace Performance;

public static class Program
{
    public static void Main(string[] args)
    {
        DefaultConfig config = new();

        if (args?.Length == 0)
        {
            // with no filter, only run these test classes
            // example: dotnet run -c Release
            BenchmarkRunner.Run<SeriesIndicators>(config);
            BenchmarkRunner.Run<BufferIndicators>(config);
            BenchmarkRunner.Run<StreamIndicators>(config);
        }
        else
        {
            // with filter, run based on arguments (e.g. filter)
            // example: dotnet run -c Release --filter *.GetAdx
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }
}
