namespace BufferList;

[TestClass]
public class EpmaBufferListTests : BufferListTestBase
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<EpmaResult> series
       = Quotes.ToEpma(lookbackPeriods);

    private static void ValidateResults(IReadOnlyList<EpmaResult> actual, IReadOnlyList<EpmaResult> expected)
    {
        actual.Should().HaveCount(expected.Count);
        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void FromReusableSplit()
    {
        EpmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        ValidateResults(sut, series);
    }

    [TestMethod]
    public void FromReusableItem()
    {
        EpmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        ValidateResults(sut, series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        EpmaList sut = new(lookbackPeriods) { reusables };

        ValidateResults(sut, series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        EpmaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        ValidateResults(sut, series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        EpmaList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<EpmaResult> expected = Quotes.ToEpma(lookbackPeriods);

        ValidateResults(sut, expected);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        EpmaList sut = new(lookbackPeriods);

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

        IReadOnlyList<EpmaResult> expected = subset.ToEpma(lookbackPeriods);

        ValidateResults(sut, expected);
    }
}
