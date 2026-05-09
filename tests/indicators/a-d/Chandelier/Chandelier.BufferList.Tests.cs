namespace BufferLists;

[TestClass]
public class Chandelier : BufferListTestBase
{
    private const int lookbackPeriods = 22;
    private const double multiplier = 3;
    private const Direction type = Direction.Long;

    private static readonly IReadOnlyList<ChandelierResult> series
       = Quotes.ToChandelier(lookbackPeriods, multiplier, type);

    [TestMethod]
    public void AddQuotes()
    {
        ChandelierList sut = new(lookbackPeriods, multiplier, type);

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
        ChandelierList sut = new(lookbackPeriods, multiplier, type) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        ChandelierList sut = new(lookbackPeriods, multiplier, type, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<ChandelierResult> expected = subset.ToChandelier(lookbackPeriods, multiplier, type);

        ChandelierList sut = new(lookbackPeriods, multiplier, type, subset);

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

        ChandelierList sut = new(lookbackPeriods, multiplier, type) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<ChandelierResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
