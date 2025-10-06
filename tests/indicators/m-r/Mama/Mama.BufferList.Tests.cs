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
        
        // Generate a continuous dataset to test pruning and calculation accuracy
        List<Quote> testQuotes = [];
        DateTime startDate = new(2020, 1, 1);
        
        // Create 1250 quotes for testing both pruning mechanisms
        for (int i = 0; i < 1250; i++)
        {
            Quote quote = Quotes[i % Quotes.Count];
            testQuotes.Add(new Quote
            {
                Timestamp = startDate.AddDays(i),
                Open = quote.Open,
                High = quote.High,
                Low = quote.Low,
                Close = quote.Close,
                Volume = quote.Volume
            });
        }
        
        // Calculate expected results for ALL quotes using static series
        // This gives us the baseline for comparison
        IReadOnlyList<MamaResult> fullExpectedResults = testQuotes.ToMama(fastLimit, slowLimit);
        
        // Create buffer list with small MaxListSize for testing result list pruning
        MamaList sut = new(fastLimit, slowLimit) { MaxListSize = 100 };

        // Add first 1200 quotes to trigger both:
        // 1. Internal state array pruning (happens at 1000 items)
        // 2. Result list pruning (happens at MaxListSize = 100)
        for (int i = 0; i < 1200; i++)
        {
            sut.Add(testQuotes[i]);
        }

        // Verify result list was pruned to stay under MaxListSize
        sut.Count.Should().BeLessThan(100);
        
        // Add the next 50 quotes after pruning and verify accuracy
        List<MamaResult> actualFinalResults = [];
        for (int i = 1200; i < 1250; i++)
        {
            sut.Add(testQuotes[i]);
            actualFinalResults.Add(sut[^1]);
        }
        
        // Compare the final 50 results against static series calculations
        // This ensures pruning didn't affect mathematical accuracy
        actualFinalResults.Count.Should().Be(50);
        
        for (int i = 0; i < actualFinalResults.Count; i++)
        {
            MamaResult actual = actualFinalResults[i];
            MamaResult expected = fullExpectedResults[1200 + i];
            
            actual.Timestamp.Should().Be(expected.Timestamp);
            
            if (expected.Mama.HasValue)
            {
                actual.Mama.Should().NotBeNull();
                actual.Mama.Should().BeApproximately(expected.Mama!.Value, 0.0001);
            }
            else
            {
                actual.Mama.Should().BeNull();
            }
            
            if (expected.Fama.HasValue)
            {
                actual.Fama.Should().NotBeNull();
                actual.Fama.Should().BeApproximately(expected.Fama!.Value, 0.0001);
            }
            else
            {
                actual.Fama.Should().BeNull();
            }
        }
    }
}
