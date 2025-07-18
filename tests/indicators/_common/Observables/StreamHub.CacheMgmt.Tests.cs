namespace Observables;

[TestClass]
public class CacheManagement : TestBase
{
    [TestMethod]
    public void Remove()
    {
        QuoteHub<Quote> provider = new();
        SmaHub<Quote> observer = provider.ToSma(20);
        provider.Add(Quotes.Take(21));

        observer.Results[19].Sma.Should().BeApproximately(214.5250, precision: DoublePrecision);

        provider.Remove(Quotes[14]);
        provider.EndTransmission();

        observer.Results[19].Sma.Should().BeApproximately(214.5260, precision: DoublePrecision);
    }

    [TestMethod]
    public void ActAddOld()  // late arrival
    {
        int length = Quotes.Count;

        // add base quotes
        QuoteHub<Quote> provider = new();

        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        // emulate incremental quotes
        for (int i = 0; i < length; i++)
        {
            // skip one
            if (i == 100)
            {
                continue;
            }

            Quote q = Quotes[i];
            provider.Add(q);
        }

        // add late
        provider.Insert(Quotes[100]);

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
        provider.EndTransmission();
    }

    [TestMethod]
    public void Overflowing()
    {
        // initialize
        QuoteHub<Quote> provider = new();

        Quote dup = new(
            Timestamp: DateTime.Now,
            Open: 1.00m,
            High: 2.00m,
            Low: 0.50m,
            Close: 1.75m,
            Volume: 1000);

        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        // overflowing, under threshold
        for (int i = 0; i <= 100; i++)
        {
            provider.Add(dup);
        }

        // assert: no fault, no overflow (yet)

        provider.Quotes.Should().HaveCount(1);
        observer.Results.Should().HaveCount(1);
        provider.IsFaulted.Should().BeFalse();
        provider.OverflowCount.Should().Be(100);
        provider.HasObservers.Should().BeTrue();

        provider.EndTransmission();
    }

    [TestMethod]
    public void OverflowedAndReset()
    {
        // initialize
        QuoteHub<Quote> provider = new();

        Quote dup = new(
            Timestamp: DateTime.Now,
            Open: 1.00m,
            High: 2.00m,
            Low: 0.50m,
            Close: 1.75m,
            Volume: 1000);

        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        // overflowed, over threshold
        Assert.ThrowsException<OverflowException>(() => {

            for (int i = 0; i <= 101; i++)
            {
                provider.Add(dup);
            }
        });

        // assert: faulted

        provider.Quotes.Should().HaveCount(1);
        observer.Results.Should().HaveCount(1);
        provider.IsFaulted.Should().BeTrue();
        provider.OverflowCount.Should().Be(101);
        provider.HasObservers.Should().BeTrue();

        // act: reset

        provider.ResetFault();

        for (int i = 0; i < 100; i++)
        {
            provider.Add(dup);
        }

        // assert: no fault, no overflow (yet)

        provider.Quotes.Should().HaveCount(1);
        observer.Results.Should().HaveCount(1);
        provider.IsFaulted.Should().BeFalse();
        provider.OverflowCount.Should().Be(100);
        provider.HasObservers.Should().BeTrue(); // not lost

        provider.EndTransmission();
    }

    [TestMethod]
    public void MaxCacheSize()
    {
        int maxCacheSize = 30;

        // initialize
        QuoteHub<Quote> provider = new(maxCacheSize);
        SmaHub<Quote> observer = provider.ToSma(20);

        // sets max cache size
        provider.MaxCacheSize.Should().Be(maxCacheSize);

        // inherits max cache size
        observer.MaxCacheSize.Should().Be(maxCacheSize);
    }

    [TestMethod]
    public void PrunedCache()
    {
        int maxCacheSize = 30;

        // initialize
        QuoteHub<Quote> provider = new(maxCacheSize);
        SmaHub<Quote> observer = provider.ToSma(20);
        IReadOnlyList<SmaResult> seriesList = Quotes.ToSma(20);

        // add quotes
        provider.Add(Quotes.Take(maxCacheSize));

        // assert: cache size is full size
        provider.Quotes.Should().HaveCount(maxCacheSize);
        observer.Results.Should().HaveCount(maxCacheSize);

        // add more quotes to exceed max cache size
        provider.Add(Quotes.Skip(maxCacheSize).Take(10));

        // assert: cache size is pruned
        provider.Results.Should().HaveCount(maxCacheSize);
        observer.Results.Should().HaveCount(maxCacheSize);

        // assert: correct values remain
        provider.Quotes.Should().BeEquivalentTo(
            Quotes.Skip(10).Take(maxCacheSize));

        observer.Results.Should().BeEquivalentTo(
            seriesList.Skip(10).Take(maxCacheSize));
    }

    [TestMethod]
    public void PrunedAsymmetric() =>
        // TODO: asymetric results (e.g. Renko)
        // pruned to correct date, instead of count
        Assert.Inconclusive("not implemented");
}
