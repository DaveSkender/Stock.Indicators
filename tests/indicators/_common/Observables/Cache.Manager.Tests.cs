namespace Tests.Common.Observables;

[TestClass]
public class CacheMgmtTests : TestBase
{
    [TestMethod]
    public void ModifyWithAnalysis() => Assert.Inconclusive("test not implemented");

    [TestMethod]
    public void ModifyWithAct() => Assert.Inconclusive("test not implemented");

    [TestMethod]
    public void Purge() => Assert.Inconclusive("test not implemented");

    [TestMethod]  // TODO: tests should include all Act enum methods
    public void ActInstructions() => Assert.Inconclusive("test not implemented");

    [TestMethod]
    public void ActAddOld()  // late arrival
    {
        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = Quotes.Count();

        // add base quotes
        QuoteHub<Quote> provider = new();

        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // emulate incremental quotes
        for (int i = 0; i < length; i++)
        {
            // skip one
            if (i == 100)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);
        }

        // add late
        provider.Add(quotesList[100]);

        // assert same as original
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            Reusable r = observer.StreamCache.Cache[i];

            // compare quote to result cache
            r.Timestamp.Should().Be(q.Timestamp);
            r.Value.Should().Be((double)q.Close);
        }

        // close observations
        provider.EndTransmission();
    }

    [TestMethod]
    public void Overflowing()
    {
        // initialize
        QuoteHub<Quote> provider = new();
        Quote q = new() { Timestamp = DateTime.Now }; // dup

        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // overflowing, under threshold
        for (int i = 0; i <= 100; i++)
        {
            provider.Add(q);
        }

        // assert: no fault, no overflow (yet)

        provider.Quotes.Should().HaveCount(1);
        observer.Results.Should().HaveCount(1);
        provider.IsFaulted.Should().BeFalse();
        provider.StreamCache.OverflowCount.Should().Be(100);
        provider.HasSubscribers.Should().BeTrue();

        provider.EndTransmission();
    }

    [TestMethod]
    public void OverflowedAndReset()
    {
        // initialize
        QuoteHub<Quote> provider = new();
        Quote q = new() { Timestamp = DateTime.Now }; // dup

        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // overflowed, over threshold
        Assert.ThrowsException<OverflowException>(() => {

            for (int i = 0; i <= 101; i++)
            {
                provider.Add(q);
            }
        });

        // assert: faulted

        provider.Quotes.Should().HaveCount(1);
        observer.Results.Should().HaveCount(1);
        provider.IsFaulted.Should().BeTrue();
        provider.StreamCache.OverflowCount.Should().Be(101);
        provider.HasSubscribers.Should().BeFalse();

        // act: reset

        provider.StreamCache.Reset();

        for (int i = 0; i < 100; i++)
        {
            provider.Add(q);
        }

        // assert: no fault, no overflow (yet)

        provider.Quotes.Should().HaveCount(1);
        observer.Results.Should().HaveCount(1);
        provider.IsFaulted.Should().BeFalse();
        provider.StreamCache.OverflowCount.Should().Be(100);
        provider.HasSubscribers.Should().BeFalse(); // expected

        provider.EndTransmission();
    }
}
