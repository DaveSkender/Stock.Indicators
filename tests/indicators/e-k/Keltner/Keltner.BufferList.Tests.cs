namespace BufferLists;

[TestClass]
public class Keltner : BufferListTestBase
{
    private const int emaPeriods = 20;
    private const double multiplier = 2;
    private const int atrPeriods = 10;

    private static readonly IReadOnlyList<KeltnerResult> series
       = Quotes.ToKeltner(emaPeriods, multiplier, atrPeriods);

    [TestMethod]
    public void AddQuotes()
    {
        KeltnerList sut = new(emaPeriods, multiplier, atrPeriods);

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
        KeltnerList sut = Quotes.ToKeltnerList(emaPeriods, multiplier, atrPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        KeltnerList sut = new(emaPeriods, multiplier, atrPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<KeltnerResult> expected = subset.ToKeltner(emaPeriods, multiplier, atrPeriods);

        KeltnerList sut = new(emaPeriods, multiplier, atrPeriods, subset);

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

        KeltnerList sut = new(emaPeriods, multiplier, atrPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<KeltnerResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
