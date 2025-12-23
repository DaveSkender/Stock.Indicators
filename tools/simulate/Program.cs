using Skender.Stock.Indicators;
using Utilities;

// define simulated quotes, arrival rate
int quotesPerMinute = 600;
int quantityToStream = 75;

List<Quote> quotes
    = Util.Setup(quantityToStream, quotesPerMinute);

// initialize quote provider

QuoteHub quoteHub = new();

// subscribe indicator hubs (SMA, EMA, etc.)

SmaHub smaHub = quoteHub.ToSmaHub(3);
EmaHub emaHub = quoteHub.ToEmaHub(5);
EmaHub useChain = quoteHub.ToQuotePartHub(CandlePart.HL2).ToEmaHub(7);
EmaHub emaChain = quoteHub.ToSmaHub(4).ToEmaHub(4);  // chainable

/* normally, you'd plugin your WebSocket here
 * and use `quoteHub.Add(q);` to connect the streams */

// simulate streaming quotes

for (int i = 0; i < quantityToStream; i++)
{
    Quote quote = quotes[i];
    quoteHub.Add(quote);  // on arrival from external WebSocket

    // govern simulation rate
    Thread.Sleep(60000 / quotesPerMinute);

    // send output to console
    Util.PrintData(quote, smaHub, emaHub, useChain, emaChain);
}
