namespace BufferLists;

[TestClass]
public class Vwma : BufferListTestBase
{
    private const int lookbackPeriods = 10;

    private static readonly IReadOnlyList<VwmaResult> series
       = Quotes.ToVwma(lookbackPeriods);

    [TestMethod]
    public override void FromQuote()
    {
        VwmaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        VwmaList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<VwmaResult> series
            = Quotes.ToVwma(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        VwmaList sut = new(lookbackPeriods);

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<VwmaResult> expected = subset.ToVwma(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void UnsupportedReusableMethods()
    {
        VwmaList sut = new(lookbackPeriods);

        // VWMA requires both price and volume, so IReusable methods should throw
        sut.Invoking(s => s.Add(DateTime.Now, 100.0))
            .Should().Throw<NotSupportedException>()
            .WithMessage("*requires both price and volume*");

        sut.Invoking(s => s.Add(new TestReusable { Timestamp = DateTime.Now, Value = 100.0 }))
            .Should().Throw<NotSupportedException>()
            .WithMessage("*requires both price and volume*");

        sut.Invoking(s => s.Add(new List<TestReusable>()))
            .Should().Throw<NotSupportedException>()
            .WithMessage("*requires both price and volume*");
    }

    private class TestReusable : IReusable
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }
}
