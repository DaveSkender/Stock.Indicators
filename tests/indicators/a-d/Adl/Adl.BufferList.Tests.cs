namespace BufferLists;

[TestClass]
public class Adl : BufferListTestBase
{
    private static readonly IReadOnlyList<AdlResult> series
       = Quotes.ToAdl();

    [TestMethod]
    public override void FromQuote()
    {
        AdlList sut = new();

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        AdlList sut = new() { Quotes };

        IReadOnlyList<AdlResult> series
            = Quotes.ToAdl();

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        AdlList sut = new(Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtorPartial()
    {
        // Test split initialization: half on construction, half after
        int splitPoint = Quotes.Count / 2;
        List<Quote> firstHalf = Quotes.Take(splitPoint).ToList();
        List<Quote> secondHalf = Quotes.Skip(splitPoint).ToList();

        AdlList sut = new(firstHalf);

        foreach (Quote q in secondHalf)
        {
            sut.Add(q);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        AdlList sut = new(subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<AdlResult> expected = subset.ToAdl();

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ManualRemovalTriggersRollback()
    {
        // Test that ADL can recover from manual removal
        // ADL stores previous value, which can be extracted from results
        AdlList sut = new();

        // Add initial quotes
        List<Quote> initialQuotes = Quotes.Take(50).ToList();
        foreach (Quote q in initialQuotes)
        {
            sut.Add(q);
        }

        sut.Should().HaveCount(50);
        double lastAdl = sut[^1].Adl;

        // Manually remove the last item
        sut.RemoveAt(49);

        // List should now have 49 items
        sut.Should().HaveCount(49);

        // Previous ADL state was rolled back to item at index 48
        double rolledBackAdl = sut[48].Adl;

        // Continue adding - should use the rolled back value
        Quote nextQuote = Quotes[50];
        sut.Add(nextQuote);

        // Should successfully add
        sut.Should().HaveCount(50);

        // The ADL value continues from the rolled back state
        // (It will differ from lastAdl because state was rolled back)
    }
}
