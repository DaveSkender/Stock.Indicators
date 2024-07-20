using Skender.Stock.Indicators;
using Utilities;

// define simulated quotes, arrival rate
int quotesPerMinute = 600;
int quantityToStream = 75;

List<Quote> quotes
    = Util.Setup(quantityToStream, quotesPerMinute);

// initialize quote provider

QuoteHub<Quote> provider = new();

// subscribe indicator hubs (SMA, EMA, etc.)

SmaHub<Quote> smaHub = provider.ToSma(3);
EmaHub<Quote> emaHub = provider.ToEma(5);
EmaHub<QuotePart> useChain = provider.ToQuotePart(CandlePart.HL2).ToEma(7);
EmaHub<SmaResult> emaChain = provider.ToSma(4).ToEma(4);  // chainable

/* normally, you'd plugin your WebSocket here 
 * and use `provider.Add(q);` to connect the streams */

// simulate streaming quotes

for (int i = 0; i < quantityToStream; i++)
{
    Quote quote = quotes[i];
    provider.Add(quote);  // on arrival from external WebSocket

    // govern simulation rate
    Thread.Sleep(60000 / quotesPerMinute);

    // send output to console
    Console.SetCursorPosition(0, i + 5);
    Util.PrintData(quote, smaHub, emaHub, useChain, emaChain);
}
