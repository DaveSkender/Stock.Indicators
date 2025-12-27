namespace BufferLists;

[TestClass]
public class Kvo : BufferListTestBase
{
    private const int fastPeriods = 34;
    private const int slowPeriods = 55;
    private const int signalPeriods = 13;

    private static readonly IReadOnlyList<KvoResult> series
       = Quotes.ToKvo(fastPeriods, slowPeriods, signalPeriods);

    [TestMethod]
    public void AddQuotes()
    {
        KvoList sut = new(fastPeriods, slowPeriods, signalPeriods);

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
        KvoList sut = Quotes.ToKvoList(fastPeriods, slowPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        KvoList sut = new(fastPeriods, slowPeriods, signalPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<KvoResult> expected = subset.ToKvo(fastPeriods, slowPeriods, signalPeriods);

        KvoList sut = new(fastPeriods, slowPeriods, signalPeriods, subset);

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

        KvoList sut = new(fastPeriods, slowPeriods, signalPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<KvoResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
