namespace BufferLists;

[TestClass]
public class ParabolicSar : BufferListTestBase, ITestQuoteBufferList
{
    private const double accelerationStep = 0.02;
    private const double maxAccelerationFactor = 0.2;

    private static readonly IReadOnlyList<ParabolicSarResult> series
        = Quotes.ToParabolicSar(accelerationStep, maxAccelerationFactor);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        ParabolicSarList sut = new(accelerationStep, maxAccelerationFactor);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        ParabolicSarList sut = Quotes.ToParabolicSarList(accelerationStep, maxAccelerationFactor);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        ParabolicSarList sut = new(
            accelerationStep,
            maxAccelerationFactor,
            accelerationStep,
            Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void Extended()
    {
        const double accelerationStepExt = 0.02;
        const double maxAccelerationFactorExt = 0.2;
        const double initialStep = 0.01;

        IReadOnlyList<ParabolicSarResult> seriesExt =
            Quotes.ToParabolicSar(accelerationStepExt, maxAccelerationFactorExt, initialStep);

        ParabolicSarList sut = new(
            accelerationStepExt,
            maxAccelerationFactorExt,
            initialStep,
            Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(seriesExt);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        ParabolicSarList sut = new(accelerationStep, maxAccelerationFactor) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<ParabolicSarResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<ParabolicSarResult> expected = subset.ToParabolicSar(accelerationStep, maxAccelerationFactor);

        ParabolicSarList sut = new(
            accelerationStep,
            maxAccelerationFactor,
            accelerationStep,
            subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }
}
