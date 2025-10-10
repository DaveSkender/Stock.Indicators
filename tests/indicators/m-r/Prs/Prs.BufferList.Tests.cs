namespace BufferLists;

[TestClass]
public class Prs : BufferListTestBase, ITestReusableBufferList
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<IReusable> evalReusables
       = OtherQuotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<IReusable> baseReusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<PrsResult> series
       = evalReusables.ToPrs(baseReusables, lookbackPeriods);

    [TestMethod]
    public void AddReusableItems()
    {
        PrsList sut = new(lookbackPeriods);

        for (int i = 0; i < evalReusables.Count; i++)
        {
            sut.Add(evalReusables[i], baseReusables[i]);
        }

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        PrsList sut = new(lookbackPeriods) {
            { evalReusables, baseReusables }
        };

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDiscreteValues()
    {
        PrsList sut = new(lookbackPeriods);

        for (int i = 0; i < evalReusables.Count; i++)
        {
            sut.Add(evalReusables[i].Timestamp, evalReusables[i].Value, baseReusables[i].Value);
        }

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotes()
    {
        // PRS uses paired series, so this test uses evalReusables and baseReusables as series
        PrsList sut = new(lookbackPeriods);

        for (int i = 0; i < evalReusables.Count; i++)
        {
            sut.Add(evalReusables[i], baseReusables[i]);
        }

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        // PRS uses paired series, so this test uses evalReusables and baseReusables as series
        PrsList sut = new(lookbackPeriods) {
            { evalReusables, baseReusables }
        };

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        // PRS uses paired series, so this test uses evalReusables and baseReusables as series
        PrsList sut = new(lookbackPeriods, evalReusables, baseReusables);

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithSeriesCtor()
    {
        PrsList sut = new(lookbackPeriods, evalReusables, baseReusables);

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void ToPrsListExtension()
    {
        PrsList sut = evalReusables.ToPrsList(baseReusables, lookbackPeriods);

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<IReusable> evalSubset = evalReusables.Take(80).ToList();
        List<IReusable> baseSubset = baseReusables.Take(80).ToList();
        IReadOnlyList<PrsResult> expected = evalSubset.ToPrs(baseSubset, lookbackPeriods);

        PrsList sut = new(lookbackPeriods, evalSubset, baseSubset);

        sut.Should().HaveCount(evalSubset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(evalSubset, baseSubset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        PrsList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(evalReusables, baseReusables);

        IReadOnlyList<PrsResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void ThrowsOnMismatchedTimestamps()
    {
        PrsList sut = new(lookbackPeriods);

        IReusable evalItem = evalReusables[0];
        IReusable baseItem = baseReusables[1]; // Different timestamp

        Action act = () => sut.Add(evalItem, baseItem);

        act.Should().Throw<InvalidQuotesException>()
            .WithMessage("*Timestamp sequence does not match*");
    }

    [TestMethod]
    public void ThrowsOnDifferentSeriesLengths()
    {
        PrsList sut = new(lookbackPeriods);

        List<IReusable> shortSeriesBase = baseReusables.Take(evalReusables.Count - 1).ToList();

        Action act = () => sut.Add(evalReusables, shortSeriesBase);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*must have the same number of items*");
    }

    [TestMethod]
    public void NoLookbackPeriods()
    {
        // Test with int.MinValue (no lookback calculation)
        IReadOnlyList<PrsResult> expectedNoLookback = evalReusables.ToPrs(baseReusables);
        PrsList sut = new(int.MinValue, evalReusables, baseReusables);

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(expectedNoLookback, options => options.WithStrictOrdering());
    }
}
