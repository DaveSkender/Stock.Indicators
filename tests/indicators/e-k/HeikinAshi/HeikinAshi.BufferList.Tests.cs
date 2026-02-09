namespace BufferLists;

[TestClass]
public class HeikinAshi : BufferListTestBase
{
    private static readonly IReadOnlyList<HeikinAshiResult> series
       = Quotes.ToHeikinAshi();

    [TestMethod]
    public void AddQuotes()
    {
        HeikinAshiList sut = [];

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
        HeikinAshiList sut = Quotes.ToHeikinAshiList();

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        HeikinAshiList sut = new(Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<HeikinAshiResult> expected = subset.ToHeikinAshi();

        HeikinAshiList sut = new(subset);

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

        HeikinAshiList sut = new() {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<HeikinAshiResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
