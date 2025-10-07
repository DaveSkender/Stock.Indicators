namespace BufferLists;

[TestClass]
public class Dpo : BufferListTestBase, ITestReusableBufferList
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<DpoResult> series
       = Quotes.ToDpo(lookbackPeriods);

    [TestMethod]
    public void AddDiscreteValues()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        // DPO results are delayed by offset periods
        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);

        // Compare the results (DPO list will be shorter due to lookahead delay)
        for (int i = 0; i < sut.Count; i++)
        {
            DpoResult expected = series[i];
            DpoResult actual = sut[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Dpo.Should().BeApproximately(expected.Dpo, 0.0001);
            actual.Sma.Should().BeApproximately(expected.Sma, 0.0001);
        }
    }

    [TestMethod]
    public void AddReusableItems()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);

        for (int i = 0; i < sut.Count; i++)
        {
            DpoResult expected = series[i];
            DpoResult actual = sut[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Dpo.Should().BeApproximately(expected.Dpo, 0.0001);
            actual.Sma.Should().BeApproximately(expected.Sma, 0.0001);
        }
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        DpoList sut = new(lookbackPeriods) { reusables };

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);

        for (int i = 0; i < sut.Count; i++)
        {
            DpoResult expected = series[i];
            DpoResult actual = sut[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Dpo.Should().BeApproximately(expected.Dpo, 0.0001);
            actual.Sma.Should().BeApproximately(expected.Sma, 0.0001);
        }
    }

    [TestMethod]
    public override void AddQuotes()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);

        for (int i = 0; i < sut.Count; i++)
        {
            DpoResult expected = series[i];
            DpoResult actual = sut[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Dpo.Should().BeApproximately(expected.Dpo, 0.0001);
            actual.Sma.Should().BeApproximately(expected.Sma, 0.0001);
        }
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        DpoList sut = Quotes.ToDpoList(lookbackPeriods);

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);

        for (int i = 0; i < sut.Count; i++)
        {
            DpoResult expected = series[i];
            DpoResult actual = sut[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Dpo.Should().BeApproximately(expected.Dpo, 0.0001);
            actual.Sma.Should().BeApproximately(expected.Sma, 0.0001);
        }
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        DpoList sut = new(lookbackPeriods, Quotes);

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);

        for (int i = 0; i < sut.Count; i++)
        {
            DpoResult expected = series[i];
            DpoResult actual = sut[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Dpo.Should().BeApproximately(expected.Dpo, 0.0001);
            actual.Sma.Should().BeApproximately(expected.Sma, 0.0001);
        }
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<DpoResult> expected = subset.ToDpo(lookbackPeriods);

        DpoList sut = new(lookbackPeriods, subset);

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = subset.Count - offset;

        sut.Should().HaveCount(expectedCount);

        for (int i = 0; i < sut.Count; i++)
        {
            DpoResult exp = expected[i];
            DpoResult actual = sut[i];

            actual.Timestamp.Should().Be(exp.Timestamp);
            actual.Dpo.Should().BeApproximately(exp.Dpo, 0.0001);
            actual.Sma.Should().BeApproximately(exp.Sma, 0.0001);
        }

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expectedCount);

        for (int i = 0; i < sut.Count; i++)
        {
            DpoResult exp = expected[i];
            DpoResult actual = sut[i];

            actual.Timestamp.Should().Be(exp.Timestamp);
            actual.Dpo.Should().BeApproximately(exp.Dpo, 0.0001);
            actual.Sma.Should().BeApproximately(exp.Sma, 0.0001);
        }
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        DpoList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        int offset = (lookbackPeriods / 2) + 1;
        int totalResults = Quotes.Count - offset;

        // After adding all quotes, we should have maxListSize items (pruned)
        sut.Should().HaveCount(Math.Min(maxListSize, totalResults));

        // Compare with the last maxListSize results from series
        int skipCount = Math.Max(0, totalResults - maxListSize);
        for (int i = 0; i < sut.Count; i++)
        {
            DpoResult expected = series[skipCount + i];
            DpoResult actual = sut[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Dpo.Should().BeApproximately(expected.Dpo, 0.0001);
            actual.Sma.Should().BeApproximately(expected.Sma, 0.0001);
        }
    }
}
