namespace BufferLists;

[TestClass]
public class Aroon : BufferListTestBase
{
    private const int lookbackPeriods = 25;

    private static readonly IReadOnlyList<AroonResult> series
        = Quotes.ToAroon(lookbackPeriods);

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        AroonList sut = new(25, Quotes);
        sut.IsBetween(static x => x.AroonUp, 0, 100);
        sut.IsBetween(static x => x.AroonDown, 0, 100);
        sut.IsBetween(static x => x.Oscillator, -100, 100);
    }

    [TestMethod]
    public void AddQuotes()
    {
        AroonList sut = new(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        AroonList sut = new(lookbackPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        AroonList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<AroonResult> expected = subset.ToAroon(lookbackPeriods);

        AroonList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        AroonList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<AroonResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
