namespace BufferLists;

[TestClass]
public class AtrStop : BufferListTestBase
{
    private const int lookbackPeriods = 21;
    private const double multiplier = 3;
    private const EndType endType = EndType.Close;

    private static readonly IReadOnlyList<AtrStopResult> series
        = Quotes.ToAtrStop(lookbackPeriods, multiplier, endType);

    [TestMethod]
    public void AddQuotes()
    {
        AtrStopList sut = new(lookbackPeriods, multiplier, endType);

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
        AtrStopList sut = new(lookbackPeriods, multiplier, endType) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        AtrStopList sut = new(lookbackPeriods, multiplier, endType, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<AtrStopResult> expected = subset.ToAtrStop(lookbackPeriods, multiplier, endType);

        AtrStopList sut = new(lookbackPeriods, multiplier, endType, subset);

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

        AtrStopList sut = new(lookbackPeriods, multiplier, endType) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<AtrStopResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
