namespace BufferLists;

[TestClass]
public class Adl : BufferListTestBase
{
    private static readonly IReadOnlyList<AdlResult> series
       = Quotes.ToAdl();

    [TestMethod]
    public override void FromQuote()
    {
        AdlList sut = new();

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        AdlList sut = new() { Quotes };

        IReadOnlyList<AdlResult> series
            = Quotes.ToAdl();

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        AdlList sut = new(Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtorPartial()
    {
        // Test split initialization: half on construction, half after
        int splitPoint = Quotes.Count / 2;
        List<Quote> firstHalf = Quotes.Take(splitPoint).ToList();
        List<Quote> secondHalf = Quotes.Skip(splitPoint).ToList();

        AdlList sut = new(firstHalf);

        foreach (Quote q in secondHalf)
        {
            sut.Add(q);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        AdlList sut = new(subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<AdlResult> expected = subset.ToAdl();

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
