using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Skender.Stock.Indicators;
using Test.Simulation;

// Parse command line arguments
string endpoint = args.Length > 0 && args[0].StartsWith("http", StringComparison.Ordinal)
    ? args[0]
    : "http://localhost:5001/quotes/random";
int interval = args.Length > 1 && int.TryParse(args[1], out int i) ? i : 100;
int count = args.Length > 2 && int.TryParse(args[2], out int c) ? c : 1000;

// Start SSE server
Process? serverProcess = ServerManager.StartServer(5001);

try
{
    // Wait for server to start
    await Task.Delay(2000).ConfigureAwait(false);

    // Run Golden Cross strategy
    using GoldenCrossStrategy strategy = new(endpoint, interval, count);
    await strategy.RunAsync().ConfigureAwait(false);
}
finally
{
    // Stop server
    ServerManager.StopServer(serverProcess);
}
