namespace BufferLists;

[TestClass]
public class ElderRay : BufferListTestBase
{
    private const int lookbackPeriods = 13;

    private static readonly IReadOnlyList<ElderRayResult> series
       = Quotes.ToElderRay(lookbackPeriods);

    [TestMethod]
    public void AddQuotes()
    {
        ElderRayList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        ElderRayList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<ElderRayResult> series
            = Quotes.ToElderRay(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        ElderRayList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        ElderRayList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<ElderRayResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<ElderRayResult> expected = subset.ToElderRay(lookbackPeriods);

        ElderRayList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }
}
