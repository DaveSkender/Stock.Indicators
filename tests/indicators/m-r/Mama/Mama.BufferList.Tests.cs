namespace BufferLists;

[TestClass]
public class Mama : BufferListTestBase
{
    private const double fastLimit = 0.5;
    private const double slowLimit = 0.05;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Use(CandlePart.HL2)  // HL2 values (not Close) for comparables
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<MamaResult> series
       = Quotes.ToMama(fastLimit, slowLimit);

    // private static readonly IReadOnlyList<MamaResult> reusableSeries
    //    = reusables.ToMama(fastLimit, slowLimit);

    [TestMethod]
    public void FromReusableSplit()
    {
        MamaList sut = new(fastLimit, slowLimit);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableItem()
    {
        MamaList sut = new(fastLimit, slowLimit);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        MamaList sut = new(fastLimit, slowLimit) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        MamaList sut = new(fastLimit, slowLimit);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        MamaList sut = new(fastLimit, slowLimit) { Quotes };

        IReadOnlyList<MamaResult> series
            = Quotes.ToMama(fastLimit, slowLimit);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        MamaList sut = new(fastLimit, slowLimit);

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<MamaResult> expected = subset.ToMama(fastLimit, slowLimit);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void AutoPruning()
    {
        // Test that MAMA's result list auto-pruning (from base class)
        // works correctly alongside its internal state array pruning
        
        // Create buffer list with small MaxListSize for testing result list pruning
        MamaList sut = new(fastLimit, slowLimit) { MaxListSize = 100 };

        // Add enough quotes to trigger both:
        // 1. Internal state array pruning (happens at 1000 items)
        // 2. Result list pruning (happens at MaxListSize = 100)
        for (int i = 0; i < 1200; i++)
        {
            Quote quote = Quotes[i % Quotes.Count];
            sut.Add(quote.Timestamp.AddDays(i), (double)(quote.High + quote.Low) / 2);
        }

        // Verify result list was pruned to stay under MaxListSize
        sut.Count.Should().BeLessThan(100);
        
        // Verify most recent results are retained and calculations still work
        MamaResult lastResult = sut[^1];
        lastResult.Should().NotBeNull();
        lastResult.Mama.Should().NotBeNull();
        lastResult.Fama.Should().NotBeNull();
        
        // Verify that the indicator still produces valid results after pruning
        // (both state array pruning and result list pruning)
        Quote newQuote = Quotes[0];
        sut.Add(newQuote.Timestamp.AddDays(1200), (double)(newQuote.High + newQuote.Low) / 2);
        
        MamaResult finalResult = sut[^1];
        finalResult.Mama.Should().NotBeNull();
        finalResult.Fama.Should().NotBeNull();
    }
}
