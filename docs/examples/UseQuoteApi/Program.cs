using Alpaca.Markets;
using Skender.Stock.Indicators;

namespace UseQuoteApi;

internal class Program
{
    private static async Task Main()
    {
        string symbol = "AAPL";

        // fetch historical quotes from data provider
        IEnumerable<Quote> quotes = await GetQuotesFromFeed(symbol);

        // calculate 10-period SMA
        IEnumerable<SmaResult> results = quotes.GetSma(10);

        if (!results.Any() || results == null)
        {
            throw new NullReferenceException("No indicator results were returned.");
        }

        // show results
        Console.WriteLine($"{symbol} Results ------- (last 10 of {results.Count()}) --");

        foreach (SmaResult r in results.TakeLast(10))
        {
            // only showing last 10 records for brevity
            Console.WriteLine($"SMA on {r.Date:u} was ${r.Sma:N3}");
        }

        // analyze results (compare to quote values)
        Console.WriteLine();
        Console.WriteLine($"{symbol} Analysis --------------------------");

        /************************************************************
          Results are usually returned with the same number of
          elements as the provided quotes; see individual indicator
          docs for more information.

          As such, converting to List means they can be indexed
          with the same ordinal position.
         ************************************************************/

        List<Quote> quotesList = quotes
            .ToList();

        List<SmaResult> resultsList = results
            .ToList();

        for (int i = quotesList.Count - 25; i < quotesList.Count; i++)
        {
            // only showing ~25 records for brevity

            Quote q = quotesList[i];
            SmaResult r = resultsList[i];

            bool isBullish = (double)q.Close > r.Sma;

            Console.WriteLine($"SMA on {r.Date:u} was ${r.Sma:N3}"
                              + $" and Bullishness is {isBullish}");
        }
    }

    private static async Task<IEnumerable<Quote>> GetQuotesFromFeed(string symbol)
    {
        /************************************************************

         We're using Alpaca SDK for .NET to access their public APIs.

         This approach will vary widely depending on where you are
         getting your quote history.

         See https://github.com/DaveSkender/Stock.Indicators/discussions/579
         for free or inexpensive market data providers and examples.

         The return type of IEnumerable<Quote> can also be List<Quote>
         or ICollection<Quote> or other IEnumerable compatible types.

         ************************************************************/

        // get and validate keys, see README.md
        string ALPACA_KEY = Environment.GetEnvironmentVariable("ALPACA_KEY");
        string ALPACA_SECRET = Environment.GetEnvironmentVariable("ALPACA_SECRET");

        if (string.IsNullOrEmpty(ALPACA_KEY))
        {
            throw new ArgumentNullException(
                ALPACA_KEY,
                $"API KEY missing, use `setx ALPACA_KEY \"MY-ALPACA-KEY\"` to set.");
        }

        if (string.IsNullOrEmpty(ALPACA_SECRET))
        {
            throw new ArgumentNullException(
                ALPACA_SECRET,
                $"API SECRET missing, use `setx AlpacaApiSecret \"MY-ALPACA-SECRET\"` to set.");
        }

        // connect to Alpaca REST API
        SecretKey secretKey = new(ALPACA_KEY, ALPACA_SECRET);

        IAlpacaDataClient client = Environments.Paper.GetAlpacaDataClient(secretKey);

        // compose request
        // (excludes last 15 minutes for free delayed quotes)
        DateTime into = DateTime.Now.Subtract(TimeSpan.FromMinutes(16));
        DateTime from = into.Subtract(TimeSpan.FromDays(1000));

        HistoricalBarsRequest request = new(symbol, from, into, BarTimeFrame.Minute);

        // fetch minute-bar quotes in Alpaca's format
        IPage<IBar> barSet = await client.ListHistoricalBarsAsync(request);

        // convert library compatible quotes
        IEnumerable<Quote> quotes = barSet
            .Items
            .Select(bar => new Quote
            {
                Date = bar.TimeUtc,
                Open = bar.Open,
                High = bar.High,
                Low = bar.Low,
                Close = bar.Close,
                Volume = bar.Volume
            })
            .OrderBy(x => x.Date); // optional

        return quotes;
    }
}
