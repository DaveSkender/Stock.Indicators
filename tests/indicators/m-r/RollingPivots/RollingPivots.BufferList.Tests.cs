namespace BufferLists;

[TestClass]
public class RollingPivots : BufferListTestBase, ITestQuoteBufferList
{
    private const int windowPeriods = 20;
    private const int offsetPeriods = 0;
    private const PivotPointType pointType = PivotPointType.Standard;

    private static readonly IReadOnlyList<RollingPivotsResult> series
       = Quotes.ToRollingPivots(windowPeriods, offsetPeriods, pointType);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        RollingPivotsList sut = new(windowPeriods, offsetPeriods, pointType);

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
        RollingPivotsList sut = new(windowPeriods, offsetPeriods, pointType) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        RollingPivotsList sut = new(windowPeriods, offsetPeriods, pointType, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<RollingPivotsResult> expected = subset.ToRollingPivots(windowPeriods, offsetPeriods, pointType);

        RollingPivotsList sut = new(windowPeriods, offsetPeriods, pointType, subset);

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

        RollingPivotsList sut = new(windowPeriods, offsetPeriods, pointType) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<RollingPivotsResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void AlternativeParameters()
    {
        const int altWindowPeriods = 10;
        const int altOffsetPeriods = 2;
        const PivotPointType altPointType = PivotPointType.Fibonacci;

        IReadOnlyList<RollingPivotsResult> expected = Quotes.ToRollingPivots(altWindowPeriods, altOffsetPeriods, altPointType);
        RollingPivotsList sut = new(altWindowPeriods, altOffsetPeriods, altPointType) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void PropertiesAreCorrect()
    {
        RollingPivotsList sut = new(windowPeriods, offsetPeriods, pointType);

        sut.WindowPeriods.Should().Be(windowPeriods);
        sut.OffsetPeriods.Should().Be(offsetPeriods);
        sut.PointType.Should().Be(pointType);
    }
}
