namespace BufferLists;

[TestClass]
public class Dema : BufferListTestBase
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<DemaResult> series
       = Quotes.ToDema(lookbackPeriods);

    [TestMethod]
    public void FromReusableSplit()
    {
        DemaList sut = new(lookbackPeriods);

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
        DemaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        DemaList sut = new(lookbackPeriods);
        sut.Add(reusables);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        DemaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        DemaList sut = new(lookbackPeriods);
        sut.Add(Quotes);

        IReadOnlyList<DemaResult> series
            = Quotes.ToDema(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        DemaList sut = new(lookbackPeriods);

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

        IReadOnlyList<DemaResult> expected = subset.ToDema(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ManualRemovalTriggersRollback()
    {
        // Test that manual removal triggers rollback for stateful indicator (DEMA)
        DemaList sut = new(lookbackPeriods);

        // Add initial quotes
        List<Quote> initialQuotes = Quotes.Take(50).ToList();
        foreach (Quote q in initialQuotes)
        {
            sut.Add(q);
        }

        sut.Should().HaveCount(50);

        // Manually remove an item
        sut.RemoveAt(25);

        // List should now have 49 items
        sut.Should().HaveCount(49);

        // Buffer state was rolled back - we can continue adding
        // State was reset, so calculations will restart
        Quote nextQuote = Quotes[50];
        sut.Add(nextQuote);

        // Should successfully add without throwing
        sut.Should().HaveCount(50);

        // The list continues to function after rollback
    }
}
