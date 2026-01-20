using Coinbase.Net;
using Coinbase.Net.Clients;
using Coinbase.Net.Objects.Models;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Skender.Stock.Indicators;

namespace Test.Simulation;

internal sealed class CoinbaseStrategy : IDisposable
{
    private readonly string _symbol;
    private readonly int _targetCount;
    private readonly CoinbaseSocketClient _socketClient;

    private readonly QuoteHub _quoteHub;
    private readonly StrategyGroup<EmaResult, EmaResult> _strategyGroup;
    private readonly EmaHub _fastEma;
    private readonly EmaHub _slowEma;

    private double _balance = 10000.0;
    private double _units;
    private double _entryValue;
    private int _totalTrades;
    private int _winningTrades;
    private int _losingTrades;
    private double _largestWin;
    private double _largestLoss;
    private readonly List<double> _tradeResults = [];

    private int _quotesProcessed;

    private const int FastPeriod = 50;
    private const int SlowPeriod = 200;

    public CoinbaseStrategy(string symbol, int targetCount)
    {
        _symbol = symbol;
        _targetCount = targetCount;
        _socketClient = new CoinbaseSocketClient();

        _quoteHub = new QuoteHub();
        _strategyGroup = new StrategyGroup<EmaResult, EmaResult>(
            _quoteHub.ToEmaHub(FastPeriod),
            _quoteHub.ToEmaHub(SlowPeriod));
        _fastEma = (EmaHub)_strategyGroup.Hub1;
        _slowEma = (EmaHub)_strategyGroup.Hub2;
    }

    public async Task RunAsync()
    {
        PrintHeader();

        try
        {
            Console.WriteLine($"[CoinbaseStrategy] Connecting to Coinbase WebSocket for {_symbol}");
            Console.WriteLine();

            TaskCompletionSource<bool> completionSource = new();

            CryptoExchange.Net.Objects.CallResult<UpdateSubscription> subscription =
                await _socketClient.AdvancedTradeApi
                    .SubscribeToTradeUpdatesAsync(
                        _symbol,
                        onData => ProcessTradeUpdate(onData.Data, completionSource),
                        ct: default)
                    .ConfigureAwait(false);

            if (!subscription.Success || subscription.Data is null)
            {
                string errorMsg = subscription.Error?.Message ?? "Unknown error";
                Console.WriteLine($"[CoinbaseStrategy] Failed to subscribe: {errorMsg}");
                Console.WriteLine($"[CoinbaseStrategy] Error code: {subscription.Error?.Code}");
                Console.WriteLine($"[CoinbaseStrategy] Error details: {subscription.Error}");
                return;
            }

            Console.WriteLine($"[CoinbaseStrategy] Successfully subscribed to {_symbol} trades");

            await completionSource.Task.ConfigureAwait(false);

            await subscription.Data.CloseAsync().ConfigureAwait(false);

            Console.WriteLine();
            PrintSummary();
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("[CoinbaseStrategy] Operation was cancelled");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[CoinbaseStrategy] Operation was cancelled");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[CoinbaseStrategy] Invalid operation: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[CoinbaseStrategy] I/O Error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _socketClient.Dispose();
    }

    private void ProcessTradeUpdate(CoinbaseTrade[] trades, TaskCompletionSource<bool> completionSource)
    {
        try
        {
            foreach (CoinbaseTrade trade in trades)
            {
                // NOTE: Using individual trade prices for all OHLC fields.
                // This is intentional for simplicity and to maximize the rate
                // of hub updates, which helps expose thread-safety issues.
                Quote quote = new(
                    Timestamp: trade.Timestamp,
                    Open: trade.Price,
                    High: trade.Price,
                    Low: trade.Price,
                    Close: trade.Price,
                    Volume: trade.Quantity);

                ProcessQuote(quote);
                _quotesProcessed++;

                if (_quotesProcessed >= _targetCount)
                {
                    completionSource.TrySetResult(true);
                    return;
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[CoinbaseStrategy] Invalid operation processing trade: {ex.Message}");
            completionSource.TrySetException(ex);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"[CoinbaseStrategy] Argument error processing trade: {ex.Message}");
            completionSource.TrySetException(ex);
        }
    }

    private void ProcessQuote(Quote quote)
    {
        _quoteHub.Add(quote);

        if (_fastEma.Results.Count < 2 || _slowEma.Results.Count < 2)
        {
            return;
        }

        bool hasPairs = _strategyGroup.TryGetBackPair(
            out BackPair<EmaResult> fastPair,
            out BackPair<EmaResult> slowPair);

        if (!hasPairs || fastPair.Current.Ema is null || slowPair.Current.Ema is null
            || fastPair.Previous.Ema is null || slowPair.Previous.Ema is null)
        {
            return;
        }

        double currentPrice = (double)quote.Close;

        bool goldenCross = fastPair.Previous.Ema <= slowPair.Previous.Ema
            && fastPair.Current.Ema > slowPair.Current.Ema;

        bool deathCross = fastPair.Previous.Ema >= slowPair.Previous.Ema
            && fastPair.Current.Ema < slowPair.Current.Ema;

        if (goldenCross && _units == 0)
        {
            _entryValue = _balance;
            _units = _balance / currentPrice;
            Console.WriteLine(
                $"{quote.Timestamp:yyyy-MM-dd HH:mm}  BUY   {_units,10:N2} @ ${currentPrice,8:N2}  " +
                $"Cost: ${_entryValue,10:N2}  Balance: $0.00");
            _balance = 0;
            _totalTrades++;
        }
        else if (deathCross && _units > 0)
        {
            double unitsSold = _units;
            double proceeds = _units * currentPrice;
            double profit = proceeds - _entryValue;
            _balance = proceeds;
            _units = 0;

            _tradeResults.Add(profit);

            if (profit > 0)
            {
                _winningTrades++;
                _largestWin = Math.Max(_largestWin, profit);
            }
            else
            {
                _losingTrades++;
                _largestLoss = Math.Min(_largestLoss, profit);
            }

            Console.WriteLine(
                $"{quote.Timestamp:yyyy-MM-dd HH:mm}  SELL  {unitsSold,10:N2} @ ${currentPrice,8:N2}  " +
                $"P&L: ${profit,10:N2}  Balance: ${_balance,10:N2}");
        }
    }

    private void PrintHeader()
    {
        Console.WriteLine("==========================================");
        Console.WriteLine("  COINBASE WEBSOCKET TRADING STRATEGY");
        Console.WriteLine("==========================================");
        Console.WriteLine($"Symbol: {_symbol}");
        Console.WriteLine($"Initial Balance: ${_balance:N2}");
        Console.WriteLine($"Fast EMA: {FastPeriod} periods");
        Console.WriteLine($"Slow EMA: {SlowPeriod} periods");
        Console.WriteLine("==========================================");
        Console.WriteLine();
        Console.WriteLine(
            "Date/Time           Action  Quantity      Price        P&L/Cost        Balance");
        Console.WriteLine(
            "-------------------------------------------------------------------------------------");
    }

    private void PrintSummary()
    {
        double finalValue;
        int resultCount = _quoteHub.Results.Count;

        if (_units > 0 && resultCount > 0)
        {
            decimal lastClose = _quoteHub.Results[resultCount - 1].Close;
            finalValue = _units * (double)lastClose;
        }
        else
        {
            finalValue = _balance;
        }

        double totalPnL = finalValue - 10000.0;

        Console.WriteLine("==========================================");
        Console.WriteLine("  STRATEGY SUMMARY");
        Console.WriteLine("==========================================");
        Console.WriteLine($"Quotes Processed:  {_quotesProcessed}");
        Console.WriteLine($"Initial Balance:   ${10000.0:N2}");
        Console.WriteLine($"Final Value:       ${finalValue:N2}");
        Console.WriteLine($"Total P&L:         ${totalPnL:N2} ({totalPnL / 10000.0:P2})");
        Console.WriteLine($"Total Trades:      {_totalTrades}");
        Console.WriteLine($"Winning Trades:    {_winningTrades}");
        Console.WriteLine($"Losing Trades:     {_losingTrades}");
        Console.WriteLine($"Largest Win:       ${_largestWin:N2}");
        Console.WriteLine($"Largest Loss:      ${_largestLoss:N2}");

        if (_tradeResults.Count > 0)
        {
            double avgPnL = _tradeResults.Average();
            Console.WriteLine($"Average P&L/Trade: ${avgPnL:N2}");
        }

        Console.WriteLine("==========================================");
    }
}
