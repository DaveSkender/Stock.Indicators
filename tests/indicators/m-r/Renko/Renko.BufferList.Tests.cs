namespace BufferLists;

[TestClass]
public class Renko : BufferListTestBase, ITestQuoteBufferList
{
    private const decimal brickSize = 1.0m;
    private const EndType endType = EndType.Close;

    private static readonly IReadOnlyList<RenkoResult> series
       = Quotes.ToRenko(brickSize, endType);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        RenkoList sut = new(brickSize, endType);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(series.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        RenkoList sut = new(brickSize, endType) { Quotes };

        sut.Should().HaveCount(series.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        RenkoList sut = new(brickSize, endType, Quotes);

        sut.Should().HaveCount(series.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        RenkoList sut = new(brickSize, endType, subset);

        sut.Should().HaveCount(subset.ToRenko(brickSize, endType).Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<RenkoResult> expected = subset.ToRenko(brickSize, endType);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 80;

        RenkoList sut = new(brickSize, endType) {
            MaxListSize = maxListSize
        };

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        IReadOnlyList<RenkoResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
