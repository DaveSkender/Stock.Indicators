# Simulation Tool

This tool simulates live trading strategies using two different approaches for streaming data:

1. **SSE (Server-Sent Events)**: Uses a local SSE server for controlled testing
2. **Coinbase WebSocket**: Uses live Coinbase WebSocket feed for real-world testing

## Purpose

This tool was created to test and reproduce thread-safety issues in StreamHubs when used with async/real-world WebSocket feeds (see [#1925](https://github.com/DaveSkender/Stock.Indicators/issues/1925)).

## Usage

### SSE Mode (Default)

Uses a local Server-Sent Events server for controlled testing:

```bash
dotnet run -- sse [endpoint] [interval] [count]

# Examples:
dotnet run -- sse                                           # Default: localhost:5001, 100ms interval, 1000 quotes
dotnet run -- sse http://localhost:5001/quotes/random 50 500 # Custom endpoint, 50ms interval, 500 quotes
```

### Coinbase WebSocket Mode

Connects to live Coinbase WebSocket feed:

```bash
dotnet run -- coinbase [symbol] [count]

# Examples:
dotnet run -- coinbase                    # Default: BTC-USD, 1000 quotes
dotnet run -- coinbase ETH-USD 500        # Ethereum, 500 quotes
dotnet run -- coinbase BTC-USD 2000       # Bitcoin, 2000 quotes
```

## Strategy

Both modes implement a Golden Cross trading strategy:

- **Fast EMA**: 50 periods
- **Slow EMA**: 200 periods
- **Buy Signal**: When Fast EMA crosses above Slow EMA
- **Sell Signal**: When Fast EMA crosses below Slow EMA
- **Initial Balance**: $10,000

## Testing Thread-Safety Issues

The Coinbase WebSocket mode is designed to reproduce the thread-safety issues reported in issue #1925:

- WebSocket events arrive asynchronously from Coinbase
- Multiple trades can arrive in quick succession
- Each trade is processed through QuoteHub and multiple indicator hubs
- This can expose race conditions in hub implementations

To test for thread-safety issues:

1. Run the Coinbase mode with a high target count
2. Monitor for exceptions (especially ArgumentOutOfRangeException)
3. Try with different symbols and counts to vary the data rate

## Dependencies

- **SSE Mode**: Requires the SSE server project (`tools/server`)
- **Coinbase Mode**: Uses JKorf.Coinbase.Net v2.11.3 (matches reported issue version)

## Notes

- The Coinbase mode requires internet connectivity
- The Coinbase WebSocket API may require authentication for some features
- The tool uses the same hub instances as would be used in production code
- Both modes exercise QuoteHub, EmaHub, and StrategyGroup
