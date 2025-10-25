namespace Observables;

[TestClass]
public class CacheManagement : TestBase
{
    [TestMethod]
    public void Remove()
    {
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);
        quoteHub.Add(Quotes.Take(21));

        observer.Results[19].Sma.Should().BeApproximately(214.5250, precision: 1e-13d); // 16 digits of precision

        quoteHub.Remove(Quotes[14]);
        quoteHub.EndTransmission();

        observer.Results[19].Sma.Should().BeApproximately(214.5260, precision: 1e-13d);

        // TODO: double-check that this floating point issue is a problem.
        // Double has 15-17 points of precision (13 decimal places for hundreds values)
        // https://learn.microsoft.com/dotnet/api/system.double
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
        quoteHub.Quotes.Should().BeEquivalentTo(
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
