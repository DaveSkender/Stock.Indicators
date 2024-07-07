using System.Runtime.InteropServices;

namespace Tests.Common.Observables;

[TestClass]
public class StackoverflowTests : TestBase
{
    [TestMethod]
    public void ManySubscribers()
    {
        // goal: test that all indicators
        // can subscribe to the same provider
        // without stack overflow

        int length = 1000;

        // setup: many random quotes (massive)
        List<Quote> quotesList
            = TestData.GetRandom(length).ToList();

        ReadOnlySpan<Quote> quotesSpan
            = CollectionsMarshal.AsSpan(quotesList);

        QuoteHub<Quote> provider = new();

        // setup: define all possible subscribers
        // TODO: add to this as more Hubs come online
        List<(string label, IEnumerable<IResult> results, bool irregular)> subscribers
            = new() {
                AddHub(provider.ToAdl()),
                AddHub(provider.ToAlligator()),
                AddHub(provider.ToEma(14)),
                AddHub(provider.ToRenko(2.1m), irregular: true)

                // SMA and USE rolled-out maximus (below)
            };

        // many SMAs
        for (int i = 1; i <= 300; i += 1)
        {
            subscribers.Add(AddHub(provider.ToSma(i)));
        }

        // all USEs
        foreach (CandlePart candlePart in Enum.GetValues<CandlePart>())
        {
            subscribers.Add(AddHub(provider.Use(candlePart)));
        }

        // act: add quotes
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesSpan[i]);
        }

        subscribers.Insert(0, new(provider.ToString(), provider.Quotes.Cast<IResult>(), false));

        // assert: this just has to not fail, really

        Console.WriteLine($"Subscribers: {subscribers.Count}");
        Console.WriteLine("--------------------");

        // assert: all non-irregular subscribers have the same count
        foreach ((string label, IEnumerable<IResult> results, bool irregular)
            in subscribers)
        {
            int resultQty = results.Count();

            Console.WriteLine($"Hub: {resultQty} - {label}");

            if (irregular) { continue; }

            resultQty.Should().Be(length);
        }

        // assert: [last subscriber] has the same dates

        ReadOnlySpan<IResult> lastSubscriber
            = CollectionsMarshal.AsSpan(subscribers[^1].results.ToList());

        for (int i = 0; i < length; i++)
        {
            Quote q = quotesSpan[i];
            IResult r = lastSubscriber[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        // act: clear provider cache (cascades to subscribers)
        int cutoff = length / 2;
        provider.ClearCache(cutoff);

        provider.Quotes.Count.Should().Be(cutoff);

        Console.WriteLine("--------------------");

        // assert: all have same count
        foreach ((string label, IEnumerable<IResult> results, bool irregular)
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
        // goal: test that a massive chain starting from
        // one provider works without stack overflow

        int length = 1000;
        int depth = 1000;

        // setup: many random quotes (massive)
        List<Quote> quotesList
            = TestData.GetRandom(length).ToList();

        ReadOnlySpan<Quote> quotesSpan
            = CollectionsMarshal.AsSpan(quotesList);

        QuoteHub<Quote> provider = new();

        // setup: subscribe a large chain depth
        List<(string label, IEnumerable<IResult> results, bool irregular)> subscribers
            = new(depth + 2);

        short increment = 5;
        int lookbackPeriods = 5;

        SmaHub<Quote> init = provider.ToSma(1);
        SmaHub<SmaResult> sma = init.ToSma(3);

        subscribers.Add(AddHub(init));
        subscribers.Add(AddHub(sma));

        for (int i = 1; i <= depth; i++)
        {
            sma = sma.ToSma(lookbackPeriods);
            subscribers.Add(AddHub(sma));

            if (lookbackPeriods >= 50) { increment = -5; }
            if (lookbackPeriods <= 5) { increment = 5; }

            lookbackPeriods += increment;
        }

        // act: add quotes
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        subscribers.Insert(0, new(provider.ToString(), provider.Quotes.Cast<IResult>(), false));

        Console.WriteLine($"Subscribers: {subscribers.Count}");
        Console.WriteLine("--------------------");

        // assert: this just has to not fail, really

        // assert: all non-irregular subscribers have the same count
        foreach ((string label, IEnumerable<IResult> results, bool irregular)
            in subscribers)
        {
            int resultQty = results.Count();
            Console.WriteLine($"Hub: {resultQty} - {label}");
            if (irregular) { continue; }
            resultQty.Should().Be(length);
        }

        // assert: [last subscriber] has the same dates

        ReadOnlySpan<IResult> lastSubscriber
            = CollectionsMarshal.AsSpan(subscribers[^1].results.ToList());

        for (int i = 0; i < length; i++)
        {
            Quote q = quotesSpan[i];
            IResult r = lastSubscriber[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        //// act: clear provider cache (cascades to subscribers)
        //int cutoff = length / 2;
        //provider.ClearCache(cutoff);

        //provider.Quotes.Count.Should().Be(cutoff);

        //// assert: all have same count
        //foreach ((string label, IEnumerable<IResult> results, bool irregular)
        //    in subscribers)
        //{
        //    int resultQty = results.Count();

        //    Console.WriteLine($"Cut: {resultQty} - {label}");

        //    if (irregular)
        //    {
        //        continue;
        //    }

        //    resultQty.Should().Be(cutoff);
        //}
    }

    private static (string, IEnumerable<IResult>, bool) AddHub<TIn, TOut>(
        IObserverHub<TIn, TOut> hub, bool irregular = false)
        where TIn : struct, ISeries
        where TOut : struct, IResult
    {
        IEnumerable<IResult> results = hub.Results.Cast<IResult>();
        return (hub.ToString(), results, irregular);
    }
}

