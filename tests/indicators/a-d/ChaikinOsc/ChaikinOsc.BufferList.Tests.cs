namespace BufferLists;

[TestClass]
public class ChaikinOsc : BufferListTestBase
{
    private const int fastPeriods = 3;
    private const int slowPeriods = 10;

    private static readonly IReadOnlyList<ChaikinOscResult> series
       = Quotes.ToChaikinOsc(fastPeriods, slowPeriods);

    [TestMethod]
    public void AddQuotes()
    {
        ChaikinOscList sut = new(fastPeriods, slowPeriods);

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
        ChaikinOscList sut = new(fastPeriods, slowPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        ChaikinOscList sut = new(fastPeriods, slowPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<ChaikinOscResult> expected = subset.ToChaikinOsc(fastPeriods, slowPeriods);

        ChaikinOscList sut = new(fastPeriods, slowPeriods, subset);

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

        ChaikinOscList sut = new(fastPeriods, slowPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<ChaikinOscResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
