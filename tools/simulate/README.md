# Simulation Tool

This tool simulates live trading strategies using two different approaches for streaming data:

1. **SSE (Server-Sent Events)**: Uses a local SSE server for controlled testing
2. **Coinbase WebSocket**: Uses live Coinbase WebSocket feed for real-world testing

## Purpose

This tool is used to test and reproduce thread-safety issues in StreamHubs when used with async/real-world WebSocket/SSE live-stream financial market feeds.

## Usage

### SSE mode (Default)

Uses a local Server-Sent Events server for controlled testing:

```bash
dotnet run -- sse [dataType] [interval] [count] [quoteInterval] [endpoint]
```

Arguments (in order):

| Argument | Type | Default | Description |
| -------- | ---- | ------- | ----------- |
| `dataType` | string | `quote` | Data type endpoint: `quote` or `trade` |
| `interval` | int | `100` | Delivery rate in milliseconds (how fast quotes are sent) |
| `count` | int | none (unlimited) | Maximum number of data points to process; omit for indefinite stream |
| `quoteInterval` | string | `1m` | Time warp: timestamp spacing between quotes (e.g., `1s`, `5m`, `1h`, `1d`) |
| `endpoint` | string | `http://localhost:5001/{dataType}/random` | SSE server endpoint URL (rarely needed) |

Examples:

```bash
dotnet run -- sse                                                    # Quote data, 100ms delivery, 1m timestamps, runs indefinitely
dotnet run -- sse quote                                              # Quote data, 100ms delivery, 1m timestamps, runs indefinitely
dotnet run -- sse quote 50                                           # Quote data, 50ms delivery, 1m timestamps, runs indefinitely
dotnet run -- sse quote 50 500                                       # Quote data, 50ms delivery, 1m timestamps, stops after 500 quotes
dotnet run -- sse quote 100 1000 1h                                  # Hourly quotes delivered every 100ms, stops after 1000 quotes
dotnet run -- sse quote 50 500 5m                                    # 5-minute quotes delivered every 50ms, stops after 500 quotes
dotnet run -- sse quote 100 0 1d                                     # Daily quotes delivered every 100ms, runs indefinitely
dotnet run -- sse trade 100 1000                                     # Trade data, 100ms delivery, stops after 1000 ticks
```

**Time warp feature**: The `quoteInterval` parameter allows fast testing of longer-term strategies without waiting real time. For example, `quoteInterval=1h` with `interval=100` delivers hourly-spaced quotes every 100ms, letting you test 24 hours of data in 2.4 seconds.

### Coinbase WebSocket mode

Connects to live Coinbase WebSocket feed using the real `JKorf.Coinbase.Net` library.

Supports three modes:

- **coinbase** (or **coinbase-klines**): 5-minute kline (candle) feed, updates ~every 5 seconds
- **coinbase-ticker**: Real-time ticker/trade feed, updates continuously

```bash
dotnet run -- coinbase [symbol] [count]
dotnet run -- coinbase-klines [symbol] [count]
dotnet run -- coinbase-ticker [symbol] [count]
```

Arguments (in order):

| Argument | Type | Default | Description |
| -------- | ---- | ------- | ----------- |
| `symbol` | string | `BTC-USD` | Coinbase trading pair (e.g., `BTC-USD`, `ETH-USD`) |
| `count` | int | none (unlimited) | Maximum number of quotes to process; omit for indefinite stream |

Examples:

```bash
dotnet run -- coinbase                      # BTC-USD klines, runs indefinitely
dotnet run -- coinbase-klines BTC-USD       # BTC-USD klines, runs indefinitely
dotnet run -- coinbase-ticker ETH-USD       # ETH-USD ticker feed, runs indefinitely
dotnet run -- coinbase BTC-USD 500          # BTC-USD klines, stops after 500 quotes
dotnet run -- coinbase-ticker ETH-USD 1000  # ETH-USD ticker, stops after 1000 quotes
```

## Strategy

Both modes implement a Golden Cross trading strategy:

- **Fast EMA**: 50 periods
- **Slow EMA**: 200 periods
- **Buy Signal**: When Fast EMA crosses above Slow EMA
- **Sell Signal**: When Fast EMA crosses below Slow EMA
- **Initial Balance**: $10,000

## Testing thread-safety issues

The Coinbase WebSocket mode is designed to reproduce the thread-safety issues:

- Subscribes to 5-minute kline (candle) updates via WebSocket
- Klines arrive approximately every 5 seconds (matching reported scenario)
- Single symbol subscription feeds a single `QuoteHub`
- Each kline is processed through `QuoteHub` and multiple indicator hubs
- This can expose race conditions in hub implementations

To test for thread-safety issues:

1. Run the Coinbase mode with a high target count
2. Monitor for exceptions (especially `ArgumentOutOfRangeException`)
3. Try with different symbols and counts to vary the data rate
4. Let it run for 30-60 seconds to match the failure window reported

## Dependencies

- **SSE Mode**: Requires the SSE server project (`tools/sse-server`)
- **Coinbase Mode**: Uses JKorf.Coinbase.Net v2.11.3 (matches reported issue version)

## Notes

- The Coinbase mode requires internet connectivity
- The Coinbase WebSocket API may require authentication for some features
- The tool uses the same hub instances as would be used in production code
- Both modes exercise QuoteHub, EmaHub, and StrategyGroup

## Stopping & cleanup

When running the simulation or SSE server locally, you may need to stop hosts manually if they do not exit cleanly.

- In the console where you started the process, press `Ctrl+C` to stop the running host.
- If the process does not stop, you can use the VS Code tasks added to the repository (see `.vscode/tasks.json`) to stop stray hosts:
  - `Stop: Simulation hosts` — stops any `dotnet` processes running the simulation project (`tools/simulate`) or `Test.Simulation` executable.
  - `Stop: SseServer hosts` — stops any `dotnet` or `Test.SseServer.exe` processes running the SSE server (`tools/sse-server`).
  - `Stop: All hosted services` — combined task that stops simulation hosts, SSE server hosts, and the VitePress dev server used by the docs site.

If you prefer to stop processes manually via PowerShell, run one of the following commands in a PowerShell terminal:

Stop Simulation hosts:

```powershell
Get-CimInstance Win32_Process | Where-Object { ($_.Name -eq 'dotnet.exe' -or $_.Name -match 'Test.Simulation') -and ($_.CommandLine -match 'tools\\simulate' -or $_.CommandLine -match 'Test.Simulation') } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force }
```

Stop SSE server hosts:

```powershell
Get-CimInstance Win32_Process | Where-Object { ($_.Name -eq 'dotnet.exe' -or $_.Name -match 'Test.SseServer') -and ($_.CommandLine -match 'tools\\sse-server' -or $_.CommandLine -match 'Test.SseServer') } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force }
```

These commands match the processes by executable name and command line to avoid killing unrelated `dotnet` instances.
