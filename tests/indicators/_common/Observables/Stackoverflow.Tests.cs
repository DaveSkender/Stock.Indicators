namespace Observables;

[TestClass]
public class StackoverflowTests : TestBase
{
    [TestMethod]
    public void FatLongStack()
    {
        // goal: about ~10 subscribers, with really long
        // quote history, checking for stack overflow

        int qtyQuotes = 20000;

        // setup: many random quotes (massive)
        List<Quote> quotesList
            = Data.GetRandom(qtyQuotes).ToList();

        QuoteHub<Quote> provider = new();

        // setup: define ~10 subscribers (flat)
        List<(string label, IEnumerable<ISeries> results, bool irregular)> subscribers
            = new() {
                HubRef(provider.ToAdl()),
                HubRef(provider.ToEma(14)) };

        // all USEs
        foreach (CandlePart candlePart in Enum.GetValues<CandlePart>())
        {
            subscribers.Add(HubRef(provider.ToQuotePart(candlePart)));
        }

        // act: add quotes
        for (int i = 0; i < qtyQuotes; i++)
        {
            provider.Add(quotesList[i]);
        }

        subscribers.Insert(0, new(
            provider.ToString(),
            provider.Quotes.Cast<ISeries>(), false));

        // assert: this just has to not fail, really

        Console.WriteLine($"Subscribers: {subscribers.Count}");
        Console.WriteLine("--------------------");

        // assert: all non-irregular subscribers have the same count
        foreach ((string label, IEnumerable<ISeries> results, bool irregular)
            in subscribers)
        {
            int resultQty = results.Count();

            Console.WriteLine($"Hub: {resultQty} - {label}");

            if (irregular) { continue; }

            resultQty.Should().Be(qtyQuotes);
        }

        // assert: [last subscriber] has the same dates

        List<ISeries> lastSubscriber
            = subscribers[^1].results.ToList();

        for (int i = 0; i < qtyQuotes; i++)
        {
            Quote q = quotesList[i];
            ISeries r = lastSubscriber[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        // act: clear provider cache (cascades to subscribers)
        int cutoff = qtyQuotes / 2;
        provider.ClearCache(cutoff);

        provider.Quotes.Count.Should().Be(cutoff);

        Console.WriteLine("--------------------");

        // assert: all have same count
        foreach ((string label, IEnumerable<ISeries> results, bool irregular)
            in subscribers)
        {
            int resultQty = results.Count();

            Console.WriteLine($"Cut: {resultQty} - {label}");

            if (irregular) { continue; }

            resultQty.Should().Be(cutoff);
        }
    }

    [TestMethod]
    public void ManyChainDepths()
    {
        // goal: test that a massive chain where each new subscriber
        // subscribes to the next creating a really long chain
        // of observers, without stack overflow

        int qtyQuotes = 10000;
        int chainDepth = 500;

        // setup: many random quotes (massive)
        List<Quote> quotesList
            = Data.GetRandom(qtyQuotes).ToList();

        QuoteHub<Quote> provider = new();

        // setup: subscribe a large chain depth
        List<(string label, IEnumerable<ISeries> results, bool irregular)> subscribers
            = new(chainDepth + 2);

        SmaHub<Quote> init = provider.ToSma(1);
        SmaHub<SmaResult> sma = init.ToSma(2);

        subscribers.Add(HubRef(init));
        subscribers.Add(HubRef(sma));

        int lookbackPeriods = 1;

        // recursive providers
        for (int i = 1; i <= chainDepth; i++)
        {
            sma = sma.ToSma(lookbackPeriods);
            subscribers.Add(HubRef(sma));

            lookbackPeriods = lookbackPeriods is 2 ? 1 : 2;
        }

        // act: add quotes
        for (int i = 0; i < qtyQuotes; i++)
        {
            provider.Add(quotesList[i]);
        }

        subscribers.Insert(0,
            new(provider.ToString(),
            provider.Quotes.Cast<ISeries>(), false));

        Console.WriteLine($"Subscribers: {subscribers.Count}");
        Console.WriteLine("--------------------");

        // assert: this just has to not fail, really

        // assert: all non-irregular subscribers have the same count
        foreach ((string label, IEnumerable<ISeries> results, bool irregular)
            in subscribers)
        {
            int resultQty = results.Count();
            Console.WriteLine($"Hub: {resultQty} - {label}");
            if (irregular) { continue; }
            resultQty.Should().Be(qtyQuotes);
        }

        // assert: [last subscriber] has the same dates

        List<ISeries> lastSubscriber
            = subscribers[^1].results.ToList();

        for (int i = 0; i < qtyQuotes; i++)
        {
            Quote q = quotesList[i];
            ISeries r = lastSubscriber[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        // act: clear provider cache (cascades to subscribers)
        int cutoff = qtyQuotes / 2;
        provider.ClearCache(cutoff);

        provider.Quotes.Count.Should().Be(cutoff);

        // assert: all have same count
        foreach ((string label, IEnumerable<ISeries> results, bool irregular)
            in subscribers)
        {
            int resultQty = results.Count();

            Console.WriteLine($"Cut: {resultQty} - {label}");

            if (irregular)
            {
                continue;
            }

            resultQty.Should().Be(cutoff);
        }
    }

    [TestMethod]
    public void ManySubscribers()
    {
        // goal: test that many indictors (all at once)
        // can subscribe to the same quote provider
        // without stack overflow; ~350 subscribers

        int qtyQuotes = 5000;

        // setup: many random quotes
        List<Quote> quotesList
            = Data.GetRandom(qtyQuotes).ToList();

        QuoteHub<Quote> provider = new();

        // setup: define all possible subscribers
        // TODO: add to this as more Hubs come online
        List<(string label, IEnumerable<ISeries> results, bool irregular)> subscribers
            = new() {
                HubRef(provider.ToAdl()),
                HubRef(provider.ToAlligator()),
                HubRef(provider.ToEma(14)),
                HubRef(provider.ToRenko(2.1m), irregular: true),
                HubRef(provider.ToQuote())
              };

        // all QuoteParts
        foreach (CandlePart candlePart in Enum.GetValues<CandlePart>())
        {
            subscribers.Add(HubRef(provider.ToQuotePart(candlePart)));
        }

        // many SMAs
        for (int i = 1; i <= 300; i++)
        {
            subscribers.Add(HubRef(provider.ToSma(i)));
        }

        // act: add quotes
        for (int i = 0; i < qtyQuotes; i++)
        {
            provider.Add(quotesList[i]);
        }

        subscribers.Insert(0, new(
            provider.ToString(),
            provider.Quotes.Cast<ISeries>(), false));

        // assert: this just has to not fail, really

        Console.WriteLine($"Subscribers: {subscribers.Count}");
        Console.WriteLine("--------------------");

        // assert: all non-irregular subscribers have the same count
        foreach ((string label, IEnumerable<ISeries> results, bool irregular)
            in subscribers)
        {
            int resultQty = results.Count();

            Console.WriteLine($"Hub: {resultQty} - {label}");

            if (irregular) { continue; }

            resultQty.Should().Be(qtyQuotes);
        }

        // assert: [last subscriber] has the same dates

        List<ISeries> lastSubscriber
            = subscribers[^1].results.ToList();

        for (int i = 0; i < qtyQuotes; i++)
        {
            Quote q = quotesList[i];
            ISeries r = lastSubscriber[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        // act: clear provider cache (cascades to subscribers)
        int cutoff = qtyQuotes / 2;
        provider.ClearCache(cutoff);

        provider.Quotes.Count.Should().Be(cutoff);

        Console.WriteLine("--------------------");

        // assert: all have same count
        foreach ((string label, IEnumerable<ISeries> results, bool irregular)
            in subscribers)
        {
            int resultQty = results.Count();

            Console.WriteLine($"Cut: {resultQty} - {label}");

            if (irregular) { continue; }

            resultQty.Should().Be(cutoff);
        }
    }

    /// <summary>
    /// Utility to get references to a hub's results.
    /// </summary>
    private static (string, IEnumerable<TOut>, bool) HubRef<TIn, TOut>(
        StreamHub<TIn, TOut> hub, bool irregular = false)
        where TIn : ISeries
        where TOut : ISeries
    {
        IEnumerable<TOut> results = hub.Cache.Cast<TOut>();
        return (hub.ToString(), results, irregular);
    }
}

