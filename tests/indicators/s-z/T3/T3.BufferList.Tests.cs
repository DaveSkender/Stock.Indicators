namespace BufferLists;

[TestClass]
public class T3 : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 5;
    private const double volumeFactor = 0.7;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<T3Result> series
       = Quotes.ToT3(lookbackPeriods, volumeFactor);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        T3List sut = new(lookbackPeriods, volumeFactor);

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
        T3List sut = new(lookbackPeriods, volumeFactor) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        T3List sut = new(lookbackPeriods, volumeFactor, Quotes);

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

        T3List sut = new(lookbackPeriods, volumeFactor, firstHalf);

        foreach (Quote q in secondHalf)
        {
            sut.Add(q);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<T3Result> expected = subset.ToT3(lookbackPeriods, volumeFactor);

        T3List sut = new(lookbackPeriods, volumeFactor, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        T3List sut = new(lookbackPeriods, volumeFactor);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        T3List sut = new(lookbackPeriods, volumeFactor) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        T3List sut = new(lookbackPeriods, volumeFactor);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        T3List sut = new(lookbackPeriods, volumeFactor) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<T3Result> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
