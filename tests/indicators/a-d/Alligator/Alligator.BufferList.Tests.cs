namespace BufferLists;

[TestClass]
public class Alligator : BufferListTestBase, ITestChainBufferList
{
    private const int jawPeriods = 13;
    private const int jawOffset = 8;
    private const int teethPeriods = 8;
    private const int teethOffset = 5;
    private const int lipsPeriods = 5;
    private const int lipsOffset = 3;

    private static readonly IReadOnlyList<IReusable> reusables
        = Quotes.ToQuotePart(CandlePart.HL2).ToList();

    private static readonly IReadOnlyList<AlligatorResult> series
        = Quotes.ToAlligator(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        AlligatorList sut = new(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);

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
        AlligatorList sut = Quotes.ToAlligatorList(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        AlligatorList sut = new(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        AlligatorList sut = new(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);

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
        AlligatorList sut = new(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        AlligatorList sut = new(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<AlligatorResult> expected = subset.ToAlligator(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);

        AlligatorList sut = new(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset, subset);

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

        AlligatorList sut = new(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<AlligatorResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
