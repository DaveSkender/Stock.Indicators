namespace BufferLists;

[TestClass]
public class Beta : BufferListTestBase, ITestNonStandardBufferListCache
{
    private const int lookbackPeriods = 20;
    private const BetaType type = BetaType.Standard;

    private static readonly IReadOnlyList<IReusable> evalReusables
        = OtherQuotes
            .Use(CandlePart.Close)
            .ToList();

    private static readonly IReadOnlyList<IReusable> mrktReusables
        = Quotes
            .Use(CandlePart.Close)
            .ToList();

    private static readonly IReadOnlyList<BetaResult> series
        = OtherQuotes.ToBeta(Quotes, lookbackPeriods, type);

    [TestMethod]
    public void AddDiscreteValues()
    {
        BetaList sut = new(lookbackPeriods, type);

        for (int i = 0; i < evalReusables.Count; i++)
        {
            sut.Add(evalReusables[i].Timestamp, evalReusables[i].Value,
                    mrktReusables[i].Timestamp, mrktReusables[i].Value);
        }

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        BetaList sut = new(lookbackPeriods, type);

        for (int i = 0; i < evalReusables.Count; i++)
        {
            sut.Add(evalReusables[i], mrktReusables[i]);
        }

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        BetaList sut = new(lookbackPeriods, type);
        sut.Add(evalReusables, mrktReusables);

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotes()
    {
        // Beta doesn't support direct quote addition as it requires two series
        // This test is not applicable for Beta
        Assert.Inconclusive("Beta requires two series inputs and doesn't support single quote addition");
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        // Beta doesn't support direct quote addition as it requires two series
        // This test is not applicable for Beta
        Assert.Inconclusive("Beta requires two series inputs and doesn't support single quote addition");
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        BetaList sut = new(lookbackPeriods, type, evalReusables, mrktReusables);

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<IReusable> evalSubset = evalReusables.Take(80).ToList();
        List<IReusable> mrktSubset = mrktReusables.Take(80).ToList();
        IReadOnlyList<BetaResult> expected = evalSubset.ToBeta(mrktSubset, lookbackPeriods, type);

        BetaList sut = new(lookbackPeriods, type, evalSubset, mrktSubset);

        sut.Should().HaveCount(evalSubset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(evalSubset, mrktSubset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        BetaList sut = new(lookbackPeriods, type) {
            MaxListSize = maxListSize
        };

        sut.Add(evalReusables, mrktReusables);

        IReadOnlyList<BetaResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AllBetaTypes()
    {
        IReadOnlyList<BetaResult> expectedAll = OtherQuotes.ToBeta(Quotes, lookbackPeriods, BetaType.All);
        BetaList sut = new(lookbackPeriods, BetaType.All, evalReusables, mrktReusables);

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(expectedAll, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void BetaUp()
    {
        IReadOnlyList<BetaResult> expectedUp = OtherQuotes.ToBeta(Quotes, lookbackPeriods, BetaType.Up);
        BetaList sut = new(lookbackPeriods, BetaType.Up, evalReusables, mrktReusables);

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(expectedUp, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void BetaDown()
    {
        IReadOnlyList<BetaResult> expectedDown = OtherQuotes.ToBeta(Quotes, lookbackPeriods, BetaType.Down);
        BetaList sut = new(lookbackPeriods, BetaType.Down, evalReusables, mrktReusables);

        sut.Should().HaveCount(evalReusables.Count);
        sut.Should().BeEquivalentTo(expectedDown, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void TimestampMismatch()
    {
        BetaList sut = new(lookbackPeriods, type);

        // Create mismatched timestamps
        DateTime evalTime = DateTime.Now;
        DateTime mrktTime = DateTime.Now.AddMinutes(1);

        Action act = () => sut.Add(evalTime, 100.0, mrktTime, 100.0);
        act.Should().Throw<InvalidQuotesException>()
           .WithMessage("*Timestamp sequence does not match*");
    }

    [TestMethod]
    public void MismatchedCounts()
    {
        List<IReusable> evalSubset = evalReusables.Take(100).ToList();
        List<IReusable> mrktSubset = mrktReusables.Take(50).ToList();

        BetaList sut = new(lookbackPeriods, type);

        Action act = () => sut.Add(evalSubset, mrktSubset);
        act.Should().Throw<InvalidQuotesException>()
           .WithMessage("*should have the same number*");
    }

    [TestMethod]
    public void AutoBufferPruning()
    {
        const int maxListSize = 200;
        const int quotesSize = 502;

        // Use test data that exceeds cache size threshold
        List<IReusable> evalSubset = evalReusables.Take(quotesSize).ToList();
        List<IReusable> mrktSubset = mrktReusables.Take(quotesSize).ToList();

        // Expected results after pruning (tail end)
        IReadOnlyList<BetaResult> expected = evalSubset
            .ToBeta(mrktSubset, lookbackPeriods, type)
            .Skip(quotesSize - maxListSize)
            .ToList();

        // Generate buffer list with pruning
        BetaList sut = new(lookbackPeriods, type, evalSubset, mrktSubset) {
            MaxListSize = maxListSize
        };

        // Verify expected results matching equivalent series values
        sut.Count.Should().Be(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
