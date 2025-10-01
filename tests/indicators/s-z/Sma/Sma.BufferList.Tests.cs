namespace BufferLists;

[TestClass]
public class Sma : BufferListTestBase
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<SmaResult> series
       = Quotes.ToSma(lookbackPeriods);

    [TestMethod]
    public void FromReusableSplit()
    {
        SmaList sut = new(lookbackPeriods);

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
        SmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        SmaList sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        SmaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        SmaList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<SmaResult> series
            = Quotes.ToSma(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        SmaList sut = new(lookbackPeriods);

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

        IReadOnlyList<SmaResult> expected = subset.ToSma(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ManualRemovalTriggersRollback()
    {
        // This test demonstrates that manual removal triggers rollback
        // and the list can continue to be used (though buffer may be cleared)
        SmaList sut = new(lookbackPeriods);

        // Add initial quotes
        List<Quote> initialQuotes = Quotes.Take(50).ToList();
        foreach (Quote q in initialQuotes)
        {
            sut.Add(q);
        }

        sut.Should().HaveCount(50);

        // Manually remove an item from the middle (not recommended, but supported)
        sut.RemoveAt(25);

        // List should now have 49 items
        sut.Should().HaveCount(49);

        // Buffer state was rolled back - we can continue adding
        // (though calculations may not be accurate until buffer refills)
        Quote nextQuote = Quotes[50];
        sut.Add(nextQuote);

        // Should successfully add without throwing
        sut.Should().HaveCount(50);

        // The list is recoverable and continues to function
        // (though historical accuracy is compromised)
    }

    [TestMethod]
    public void ManualRemoveRangeHandledGracefully()
    {
        // Test that removing multiple items is handled
        SmaList sut = new(lookbackPeriods);

        // Add quotes
        List<Quote> quotes = Quotes.Take(100).ToList();
        foreach (Quote q in quotes)
        {
            sut.Add(q);
        }

        // Get a reference to an item to remove
        SmaResult itemToRemove = sut[50];

        // Remove using List<T>.Remove method
        bool removed = sut.Remove(itemToRemove);

        removed.Should().BeTrue();
        sut.Should().HaveCount(99);

        // Can still add more data
        Quote newQuote = Quotes[100];
        sut.Add(newQuote);

        sut.Should().HaveCount(100);
    }
}
