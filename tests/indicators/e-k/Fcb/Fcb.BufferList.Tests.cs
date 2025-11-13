namespace BufferLists;

[TestClass]
public class Fcb : BufferListTestBase
{
    private const int windowSpan = 2;

    private static readonly IReadOnlyList<FcbResult> series
       = Quotes.ToFcb(windowSpan);

    [TestMethod]
    public void AddQuotes()
    {
        FcbList sut = new(windowSpan);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        FcbList sut = new(windowSpan) { Quotes };

        IReadOnlyList<FcbResult> series
            = Quotes.ToFcb(windowSpan);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        FcbList sut = new(windowSpan, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        FcbList sut = new(windowSpan) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<FcbResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<FcbResult> expected = subset.ToFcb(windowSpan);

        FcbList sut = new(windowSpan, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
