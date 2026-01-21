using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Skender.Stock.Indicators;
using Test.Simulation;

// Parse command line arguments
string mode = args.Length > 0 ? args[0].ToUpperInvariant() : "SSE";

if (mode == "COINBASE" || mode == "COINBASE-TICKER" || mode == "COINBASE-KLINES")
{
    // Coinbase WebSocket mode
    CoinbaseMode coinbaseMode = mode == "COINBASE-TICKER" ? CoinbaseMode.Ticker : CoinbaseMode.Klines;
    string symbol = args.Length > 1 ? args[1] : "BTC-USD";
    int count = args.Length > 2 && int.TryParse(args[2], out int c) ? c : int.MaxValue;

    Console.WriteLine($"Starting Coinbase WebSocket simulation ({coinbaseMode})...");
    Console.WriteLine($"Symbol: {symbol}, Target count: {count}");
    Console.WriteLine();

    using CoinbaseStrategy strategy = new(symbol, count, coinbaseMode);
    await strategy.RunAsync().ConfigureAwait(false);
}
else
{
    // SSE mode (default)
    string dataType = args.Length > 1 ? args[1].ToUpperInvariant() : "QUOTE";
    int interval = args.Length > 2 && int.TryParse(args[2], out int i) ? i : 100;
    int count = args.Length > 3 && int.TryParse(args[3], out int c) ? c : int.MaxValue;
    string quoteInterval = args.Length > 4 ? args[4] : "1m";

    // Build default endpoint URL from dataType (normalize to lowercase for URL)
#pragma warning disable CA1308 // ToLowerInvariant is intentional for URL construction
    string defaultEndpoint = dataType switch {
        "QUOTE" => "http://localhost:5001/quotes/random",
        "TRADE" => "http://localhost:5001/trades/random",
        _ => $"http://localhost:5001/{dataType.ToUpperInvariant()}s/random"
            .ToLowerInvariant()
    };
#pragma warning restore CA1308

    string endpoint = args.Length > 5 && !string.IsNullOrWhiteSpace(args[5]) && args[5].StartsWith("http", StringComparison.Ordinal)
        ? args[5]
        : defaultEndpoint;

    // Start SSE server
    Process? serverProcess = ServerManager.StartServer(5001);

    try
    {
        // Wait for server to start
        Console.WriteLine("[Program] Waiting for SSE server to be ready...");
        await Task.Delay(3000).ConfigureAwait(false);
        Console.WriteLine("[Program] Starting strategy...");
        Console.WriteLine();

        // Run Golden Cross strategy
        using GoldenCrossStrategy strategy = new(endpoint, interval, count, quoteInterval);
        await strategy.RunAsync().ConfigureAwait(false);
    }
    finally
    {
        // Stop server
        ServerManager.StopServer(serverProcess);
    }
}
