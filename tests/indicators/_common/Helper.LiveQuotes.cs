using Alpaca.Markets;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

internal class LiveData
{
    internal static async Task<IEnumerable<Quote>> GetQuotesFromFeed(
        string symbol,
        int days = 500)
    {
        /************************************************************

         We're using Alpaca SDK for .NET to access their public APIs.

         This approach will vary widely depending on where you are
         getting your quote history.

         See https://github.com/DaveSkender/Stock.Indicators/discussions/579
         for free or inexpensive market data providers and examples.

         ************************************************************/

        // get and validate keys, see README.md
        string alpacaApiKey = Environment.GetEnvironmentVariable("AlpacaApiKey");
        string alpacaSecret = Environment.GetEnvironmentVariable("AlpacaSecret");

        ArgumentException.ThrowIfNullOrEmpty(nameof(alpacaApiKey));
        ArgumentException.ThrowIfNullOrEmpty(nameof(alpacaSecret));

        // connect to Alpaca REST API
        SecretKey secretKey = new(alpacaApiKey, alpacaSecret);

        IAlpacaDataClient client = Environments
            .Paper
            .GetAlpacaDataClient(secretKey);

        // compose request
        // (excludes last 15 minutes for free delayed quotes)
        DateTime into = DateTime.Now.Subtract(TimeSpan.FromMinutes(16));
        DateTime from = into.Subtract(TimeSpan.FromDays(days));

        HistoricalBarsRequest request = new(symbol, from, into, BarTimeFrame.Day);

        // fetch minute-bar quotes in Alpaca's format
        IPage<IBar> barSet = await client
            .ListHistoricalBarsAsync(request)
            .ConfigureAwait(false);

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
            .OrderBy(x => x.Date);

        return quotes;
    }
}
