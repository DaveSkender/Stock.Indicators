using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Skender.Stock.Indicators;
using Test.Simulation;

// Parse command line arguments
string mode = args.Length > 0 ? args[0].ToUpperInvariant() : "SSE";

if (mode == "COINBASE")
{
    // Coinbase WebSocket mode
    string symbol = args.Length > 1 ? args[1] : "BTC-USD";
    int count = args.Length > 2 && int.TryParse(args[2], out int c) ? c : 1000;

    Console.WriteLine("Starting Coinbase WebSocket simulation...");
    Console.WriteLine($"Symbol: {symbol}, Target count: {count}");
    Console.WriteLine();

    using CoinbaseStrategy strategy = new(symbol, count);
    await strategy.RunAsync().ConfigureAwait(false);
}
else
{
    // SSE mode (default)
    string endpoint = args.Length > 1 && args[1].StartsWith("http", StringComparison.Ordinal)
        ? args[1]
        : "http://localhost:5001/quotes/random";
    int interval = args.Length > 2 && int.TryParse(args[2], out int i) ? i : 100;
    int count = args.Length > 3 && int.TryParse(args[3], out int c) ? c : 1000;

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
}
