namespace BufferLists;

[TestClass]
public class ParabolicSar : BufferListTestBase
{
    private const double accelerationStep = 0.02;
    private const double maxAccelerationFactor = 0.2;

    private static readonly IReadOnlyList<ParabolicSarResult> series
       = Quotes.ToParabolicSar(accelerationStep, maxAccelerationFactor);

    [TestMethod]
    public override void AddQuotes()
    {
        ParabolicSarList sut = new(accelerationStep, maxAccelerationFactor);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        ParabolicSarList sut = Quotes.ToParabolicSarList(accelerationStep, maxAccelerationFactor);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        ParabolicSarList sut = new(accelerationStep, maxAccelerationFactor, accelerationStep, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithExplicitInitialFactor()
    {
        const double initialFactor = 0.03;

        IReadOnlyList<ParabolicSarResult> expected = Quotes.ToParabolicSar(
            accelerationStep, maxAccelerationFactor, initialFactor);

        ParabolicSarList sut = Quotes.ToParabolicSarList(
            accelerationStep, maxAccelerationFactor, initialFactor);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<ParabolicSarResult> expected = subset.ToParabolicSar(accelerationStep, maxAccelerationFactor);

        ParabolicSarList sut = new(accelerationStep, maxAccelerationFactor, accelerationStep, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        ParabolicSarList sut = new(accelerationStep, maxAccelerationFactor) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<ParabolicSarResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
