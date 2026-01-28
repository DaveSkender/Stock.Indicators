namespace BufferLists;

[TestClass]
public class Fractal : BufferListTestBase, ITestQuoteBufferList
{
    private const int windowSpan = 2;
    private const EndType endType = EndType.HighLow;

    private static readonly IReadOnlyList<FractalResult> series
       = Quotes.ToFractal(windowSpan, endType);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        FractalList sut = new(windowSpan, endType);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        FractalList sut = new(windowSpan, endType) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        FractalList sut = new(windowSpan, Quotes, endType);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<FractalResult> expected = subset.ToFractal(windowSpan, endType);

        FractalList sut = new(windowSpan, subset, endType);

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

        FractalList sut = new(windowSpan, endType) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<FractalResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void DifferentSpans_ProducesCorrectResults()
    {
        const int leftSpan = 2;
        const int rightSpan = 4;

        IReadOnlyList<FractalResult> expected = Quotes.ToFractal(leftSpan, rightSpan, endType);
        FractalList sut = new(leftSpan, rightSpan, Quotes, endType);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void CloseEndType_ProducesCorrectResults()
    {
        const EndType closeType = EndType.Close;

        IReadOnlyList<FractalResult> expected = Quotes.ToFractal(windowSpan, closeType);
        FractalList sut = new(windowSpan, Quotes, closeType);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(expected);
    }
}
