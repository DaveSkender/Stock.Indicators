using System.Globalization;
using System.Text.Json;
using Skender.Stock.Indicators;

namespace Test.Simulation;

internal sealed class GoldenCrossStrategy : IDisposable
{
    private readonly string _endpoint;
    private readonly int _interval;
    private readonly int _targetCount;
    private readonly HttpClient _httpClient;

    private readonly QuoteHub _quoteHub;
    private readonly StrategyHub<IQuote> _strategyHub;
    private readonly EmaHub _fastEma;
    private readonly EmaHub _slowEma;

    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    private double _balance = 10000.0;
    private double _units;
    private int _totalTrades;
    private int _winningTrades;
    private int _losingTrades;
    private double _largestWin;
    private double _largestLoss;
    private readonly List<double> _tradeResults = [];

    private const int FastPeriod = 50;
    private const int SlowPeriod = 200;

    public GoldenCrossStrategy(string endpoint, int interval, int targetCount)
    {
        _endpoint = endpoint;
        _interval = interval;
        _targetCount = targetCount;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };

        _quoteHub = new QuoteHub();
        _strategyHub = new StrategyHub<IQuote>(_quoteHub);
        _fastEma = _strategyHub.Use<EmaHub, EmaResult>(_quoteHub.ToEmaHub(FastPeriod));
        _slowEma = _strategyHub.Use<EmaHub, EmaResult>(_quoteHub.ToEmaHub(SlowPeriod));
    }

    public async Task RunAsync()
    {
        PrintHeader();

        try
        {
            Uri uri = new($"{_endpoint}?interval={_interval}&batchSize={_targetCount}");
            Console.WriteLine($"[Strategy] Connecting to {uri}");
            Console.WriteLine();

            using HttpResponseMessage response = await _httpClient
                .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            await using Stream stream = await response.Content
                .ReadAsStreamAsync()
                .ConfigureAwait(false);
            using StreamReader reader = new(stream);

            int quotesProcessed = 0;
            string? line;

            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
            {
                // SSE format: "event: quote", "data: {...}", blank line
                if (line.StartsWith("data:", StringComparison.Ordinal))
                {
                    string json = line[5..].Trim();
                    Quote? quote = JsonSerializer.Deserialize<Quote>(json, JsonOptions);

                    if (quote is not null)
                    {
                        ProcessQuote(quote);
                        quotesProcessed++;

                        if (quotesProcessed >= _targetCount)
                        {
                            break;
                        }
                    }
                }
            }

            Console.WriteLine();
            PrintSummary();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[Strategy] HTTP Error: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Strategy] I/O Error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Strategy] JSON Error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private void ProcessQuote(Quote quote)
    {
        _quoteHub.Add(quote);

        // Need at least 2 results to detect crossovers
        if (_fastEma.Results.Count < 2 || _slowEma.Results.Count < 2)
        {
            return;
        }

        bool hasPairs = _strategyHub.TryGetLatest(
            _fastEma,
            _slowEma,
            out (EmaResult previous, EmaResult current) fastPair,
            out (EmaResult previous, EmaResult current) slowPair);

        if (!hasPairs || fastPair.current.Ema is null || slowPair.current.Ema is null
            || fastPair.previous.Ema is null || slowPair.previous.Ema is null)
        {
            return;
        }

        double currentPrice = (double)quote.Close;

        // Golden Cross: Fast EMA crosses above Slow EMA (Buy signal)
        bool goldenCross = fastPair.previous.Ema <= slowPair.previous.Ema
            && fastPair.current.Ema > slowPair.current.Ema;

        // Death Cross: Fast EMA crosses below Slow EMA (Sell signal)
        bool deathCross = fastPair.previous.Ema >= slowPair.previous.Ema
            && fastPair.current.Ema < slowPair.current.Ema;

        if (goldenCross && _units == 0)
        {
            // Buy
            _units = _balance / currentPrice;
            double cost = _units * currentPrice;
            Console.WriteLine(
                $"{quote.Timestamp:yyyy-MM-dd HH:mm}  BUY   {_units,10:N2} @ ${currentPrice,8:N2}  " +
                $"Cost: ${cost,10:N2}  Balance: ${_balance,10:N2}");
            _totalTrades++;
        }
        else if (deathCross && _units > 0)
        {
            // Sell
            double proceeds = _units * currentPrice;
            double profit = proceeds - _balance;
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
                $"{quote.Timestamp:yyyy-MM-dd HH:mm}  SELL  {_units,10:N2} @ ${currentPrice,8:N2}  " +
                $"P&L: ${profit,10:N2}  Balance: ${_balance,10:N2}");
        }
    }

    private void PrintHeader()
    {
        Console.WriteLine("==========================================");
        Console.WriteLine("  GOLDEN CROSS TRADING STRATEGY");
        Console.WriteLine("==========================================");
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
        double finalValue = _units > 0 ? _units * (double)_quoteHub.Results[^1].Close : _balance;
        double totalPnL = finalValue - 10000.0;

        Console.WriteLine("==========================================");
        Console.WriteLine("  STRATEGY SUMMARY");
        Console.WriteLine("==========================================");
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
