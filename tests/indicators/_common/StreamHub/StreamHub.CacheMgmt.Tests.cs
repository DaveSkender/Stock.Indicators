namespace Observables;

[TestClass]
public class CacheManagement : TestBase
{
    [TestMethod]
    public void Remove()
    {
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);

        List<Quote> quotes = Quotes.Take(21).ToList();

        quoteHub.Add(quotes);

        Console.WriteLine(observer.Results.ToStringOut());

        // Verify StreamHub matches Series for same input
        IReadOnlyList<SmaResult> seriesBeforeRemove = quotes.ToSma(20);
        observer.Results[19].Sma.Should().Be(seriesBeforeRemove[19].Sma);

        // Create new quote list with the removed item (more efficient than LINQ Where)
        List<Quote> quotesAfterRemove = [.. quotes];
        quotesAfterRemove.RemoveAt(14);

        quoteHub.Remove(Quotes[14]);
        quoteHub.EndTransmission();

        Console.WriteLine(observer.Results.ToStringOut());

        // After removal, we have 20 quotes, period is 20, so SMA starts at index 19
        // StreamHub result at index 19 should match Series result at index 19 (last element)
        IReadOnlyList<SmaResult> seriesAfterRemove = quotesAfterRemove.ToSma(20);
        observer.Results[19].Sma.Should().Be(seriesAfterRemove[19].Sma);
    }

    /// <summary>
    /// late arrival
    /// </summary>
    [TestMethod]
    public void ActAddOld()
    {
        int length = Quotes.Count;

        // add base quotes
        QuoteHub quoteHub = new();

        QuotePartHub observer = quoteHub
            .ToQuotePartHub(CandlePart.Close);

        // emulate incremental quotes
        for (int i = 0; i < length; i++)
        {
            // skip one
            if (i == 100)
            {
                continue;
            }

            Quote q = Quotes[i];
            quoteHub.Add(q);
        }

        // add late
        quoteHub.Insert(Quotes[100]);

        // assert same as original
        for (int i = 0; i < length; i++)
        {
            Quote q = Quotes[i];
            QuotePart r = observer.Cache[i];

            // compare quote to result cache
            r.Timestamp.Should().Be(q.Timestamp);
            r.Value.Should().Be(q.Value);
        }

        // close observations
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Overflowing()
    {
        // initialize
        QuoteHub quoteHub = new();

        Quote dup = new(
            Timestamp: DateTime.Now,
            Open: 1.00m,
            High: 2.00m,
            Low: 0.50m,
            Close: 1.75m,
            Volume: 1000);

        QuotePartHub observer = quoteHub
            .ToQuotePartHub(CandlePart.Close);

        // overflowing, under threshold
        for (int i = 0; i <= 100; i++)
        {
            quoteHub.Add(dup);
        }

        // assert: no fault, no overflow (yet)

        quoteHub.Quotes.Should().HaveCount(1);
        observer.Results.Should().HaveCount(1);
        quoteHub.IsFaulted.Should().BeFalse();
        quoteHub.OverflowCount.Should().Be(100);
        quoteHub.HasObservers.Should().BeTrue();

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void OverflowedAndReset()
    {
        // initialize
        QuoteHub quoteHub = new();

        Quote dup = new(
            Timestamp: DateTime.Now,
            Open: 1.00m,
            High: 2.00m,
            Low: 0.50m,
            Close: 1.75m,
            Volume: 1000);

        QuotePartHub observer = quoteHub
            .ToQuotePartHub(CandlePart.Close);

        // overflowed, over threshold
        Assert.ThrowsExactly<OverflowException>(
            () => {

                for (int i = 0; i <= 101; i++)
                {
                    quoteHub.Add(dup);
                }
            });

        // assert: faulted

        quoteHub.Quotes.Should().HaveCount(1);
        observer.Results.Should().HaveCount(1);
        quoteHub.IsFaulted.Should().BeTrue();
        quoteHub.OverflowCount.Should().Be(101);
        quoteHub.HasObservers.Should().BeTrue();

        // act: reset

        quoteHub.ResetFault();

        for (int i = 0; i < 100; i++)
        {
            quoteHub.Add(dup);
        }

        // assert: no fault, no overflow (yet)

        quoteHub.Quotes.Should().HaveCount(1);
        observer.Results.Should().HaveCount(1);
        quoteHub.IsFaulted.Should().BeFalse();
        quoteHub.OverflowCount.Should().Be(100);
        quoteHub.HasObservers.Should().BeTrue(); // not lost

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void MaxCacheSize()
    {
        const int maxCacheSize = 30;

        // initialize
        QuoteHub quoteHub = new(maxCacheSize);
        SmaHub observer = quoteHub.ToSmaHub(20);

        // sets max cache size
        quoteHub.MaxCacheSize.Should().Be(maxCacheSize);

        // inherits max cache size
        observer.MaxCacheSize.Should().Be(maxCacheSize);
    }

    [TestMethod]
    public void PrunedCache()
    {
        const int maxCacheSize = 30;

        // initialize
        QuoteHub quoteHub = new(maxCacheSize);
        SmaHub observer = quoteHub.ToSmaHub(20);
        IReadOnlyList<SmaResult> seriesList = Quotes.ToSma(20);

        // add quotes
        quoteHub.Add(Quotes.Take(maxCacheSize));

        // assert: cache size is full size
        quoteHub.Quotes.Should().HaveCount(maxCacheSize);
        observer.Results.Should().HaveCount(maxCacheSize);

        // add more quotes to exceed max cache size
        quoteHub.Add(Quotes.Skip(maxCacheSize).Take(10));

        // assert: cache size is pruned
        quoteHub.Results.Should().HaveCount(maxCacheSize);
        observer.Results.Should().HaveCount(maxCacheSize);

        // assert: correct values remain
        quoteHub.Quotes.IsExactly(
            Quotes.Skip(10).Take(maxCacheSize));

        observer.Results.IsExactly(
            seriesList.Skip(10).Take(maxCacheSize));
    }

    [TestMethod]
    public void PrunedAsymmetric()
    {
        const int maxCacheSize = 30;
        const decimal brickSize = 2.5m;
        const EndType endType = EndType.Close;

        // initialize
        QuoteHub quoteHub = new(maxCacheSize);
        RenkoHub observer = quoteHub.ToRenkoHub(brickSize, endType);
        IReadOnlyList<RenkoResult> seriesList = Quotes.ToRenko(brickSize, endType);

        // add quotes (Renko produces asymmetric results - can be 0 or many bricks per quote)
        quoteHub.Add(Quotes.Take(maxCacheSize));

        // assert: cache size is at or under max (Renko may produce fewer results than quotes)
        quoteHub.Quotes.Should().HaveCount(maxCacheSize);
        observer.Results.Should().HaveCountLessThanOrEqualTo(maxCacheSize);

        // add more quotes to exceed max cache size
        quoteHub.Add(Quotes.Skip(maxCacheSize).Take(10));

        // assert: quote cache is pruned to max size
        quoteHub.Quotes.Should().HaveCount(maxCacheSize);

        // assert: Renko cache is pruned by date, not count
        // (should contain all Renko bricks from the most recent maxCacheSize quotes)
        DateTime oldestQuoteDate = quoteHub.Quotes[0].Timestamp;
        observer.Results.Should().OnlyContain(r => r.Timestamp >= oldestQuoteDate,
            "Renko bricks should be pruned by date to match the oldest quote in cache");

        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Verifies that exposed cache references cannot be cast to mutable lists.
    /// This prevents users from bypassing safe StreamHub methods.
    /// </summary>
    [TestMethod]
    public void CacheReferencesAreImmutable()
    {
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);

        List<Quote> quotes = Quotes.Take(25).ToList();
        quoteHub.Add(quotes);

        // verify Results cannot be cast to mutable list
        IReadOnlyList<SmaResult> results = observer.Results;
        bool canCastResults = results is List<SmaResult>;
        canCastResults.Should().BeFalse("Results should not be castable to List<T>");

        // verify GetCacheRef cannot be cast to mutable list
        IReadOnlyList<SmaResult> cacheRef = observer.GetCacheRef();
        bool canCastCacheRef = cacheRef is List<SmaResult>;
        canCastCacheRef.Should().BeFalse("GetCacheRef() should not be castable to List<T>");

        // verify QuoteHub.Quotes cannot be cast to mutable list
        IReadOnlyList<IQuote> quotesRef = quoteHub.Quotes;
        bool canCastQuotes = quotesRef is List<IQuote>;
        canCastQuotes.Should().BeFalse("Quotes should not be castable to List<T>");

        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Verifies that adding a quote with the same timestamp replaces the existing quote
    /// instead of clearing the cache (standalone QuoteHub vulnerability fix).
    /// </summary>
    [TestMethod]
    public void UpdateQuoteWithSameTimestamp()
    {
        QuoteHub quoteHub = new();
        QuotePartHub observer = quoteHub.ToQuotePartHub(CandlePart.Close);

        DateTime timestamp = new(2020, 1, 1, 10, 0, 0);

        // add initial quote
        Quote q1 = new(timestamp, 100m, 105m, 95m, 102m, 1000);
        quoteHub.Add(q1);

        quoteHub.Quotes.Should().HaveCount(1);
        quoteHub.Quotes[0].Close.Should().Be(102m);
        observer.Results.Should().HaveCount(1);
        observer.Results[0].Value.Should().Be(102);

        // add updated quote with same timestamp but different values
        // should replace the existing quote and notify observers to rebuild
        Quote q2 = new(timestamp, 100m, 110m, 90m, 108m, 1500);
        quoteHub.Add(q2);

        // QuoteHub should still have 1 quote with updated values
        quoteHub.Quotes.Should().HaveCount(1);
        quoteHub.Quotes[0].Close.Should().Be(108m);

        // observer should rebuild from QuoteHub's cache with updated values
        observer.Results.Should().HaveCount(1);
        observer.Results[0].Value.Should().Be(108);

        quoteHub.EndTransmission();
    }
}
