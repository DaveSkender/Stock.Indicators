namespace BufferLists;

[TestClass]
public class Adl : BufferListTestBase
{
    private static readonly IReadOnlyList<AdlResult> series
       = Quotes.ToAdl();

    [TestMethod]
    public void AddQuotes()
    {
        AdlList sut = [];

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
        AdlList sut = new() { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        AdlList sut = new(Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtorPartial()
    {
        // Test split initialization: half on construction, half after
        int splitPoint = Quotes.Count / 2;
        List<Quote> firstHalf = Quotes.Take(splitPoint).ToList();
        List<Quote> secondHalf = Quotes.Skip(splitPoint).ToList();

        AdlList sut = new(firstHalf);

        foreach (Quote quote in secondHalf)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<AdlResult> expected = subset.ToAdl();

        AdlList sut = new(subset);

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

        AdlList sut = new() {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<AdlResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
