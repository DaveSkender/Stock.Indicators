using BenchmarkDotNet.Running;

[assembly: CLSCompliant(false)]

namespace Performance;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args?.Length == 0)
        {
            // run all
            // example: dotnet run -c Release
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
        else
        {
            // run based on arguments (e.g. filter)
            // example: dotnet run -c Release --filter *.GetAdx
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }

    /* USAGE
     * 
     * dotnet build -c Release
     * 
     * Examples, to run cohorts:
     * dotnet run -c Release -filter *Stream*
     * dotnet run -c Release -filter *External.EmaHub*
     */
}
