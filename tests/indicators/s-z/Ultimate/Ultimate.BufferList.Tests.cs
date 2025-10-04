namespace BufferLists;

[TestClass]
public class Ultimate : BufferListTestBase
{
    private const int shortPeriods = 7;
    private const int middlePeriods = 14;
    private const int longPeriods = 28;

    private static readonly IReadOnlyList<UltimateResult> series
       = Quotes.ToUltimate(shortPeriods, middlePeriods, longPeriods);

    [TestMethod]
    public override void FromQuote()
    {
        UltimateList sut = new(shortPeriods, middlePeriods, longPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        UltimateList sut = new(shortPeriods, middlePeriods, longPeriods) { Quotes };

        IReadOnlyList<UltimateResult> series
            = Quotes.ToUltimate(shortPeriods, middlePeriods, longPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        UltimateList sut = new(shortPeriods, middlePeriods, longPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        UltimateList sut = new(shortPeriods, middlePeriods, longPeriods, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<UltimateResult> expected = subset.ToUltimate(shortPeriods, middlePeriods, longPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
