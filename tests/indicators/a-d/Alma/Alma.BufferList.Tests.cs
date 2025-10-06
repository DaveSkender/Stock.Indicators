namespace BufferLists;

[TestClass]
public class Alma : BufferListTestBase
{
    private const int lookbackPeriods = 10;
    private const double offset = 0.85;
    private const double sigma = 6;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<AlmaResult> series
       = Quotes.ToAlma(lookbackPeriods, offset, sigma);

    [TestMethod]
    public void FromReusableSplit()
    {
        AlmaList sut = new(lookbackPeriods, offset, sigma);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableItem()
    {
        AlmaList sut = new(lookbackPeriods, offset, sigma);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        AlmaList sut = new(lookbackPeriods, offset, sigma) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        AlmaList sut = new(lookbackPeriods, offset, sigma);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        AlmaList sut = new(lookbackPeriods, offset, sigma) { Quotes };

        IReadOnlyList<AlmaResult> series
            = Quotes.ToAlma(lookbackPeriods, offset, sigma);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        AlmaList sut = new(lookbackPeriods, offset, sigma, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        AlmaList sut = new(lookbackPeriods, offset, sigma, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<AlmaResult> expected = subset.ToAlma(lookbackPeriods, offset, sigma);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void AlmaListViaReusable()
    {
        // arrange
        IReadOnlyList<AlmaResult> expected = Quotes.ToAlma(lookbackPeriods, offset, sigma);

        // act - using IReusable interface
        AlmaList actual = new(lookbackPeriods, offset, sigma) {
            Quotes.Use(CandlePart.Close)
        };

        // assert
        actual.Should().HaveCount(expected.Count);
        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void AlmaListViaTimestampValue()
    {
        // arrange
        IReadOnlyList<AlmaResult> expected = Quotes.ToAlma(lookbackPeriods, offset, sigma);

        // act - using timestamp/value method
        AlmaList actual = new(lookbackPeriods, offset, sigma);
        for (int i = 0; i < Quotes.Count; i++)
        {
            actual.Add(Quotes[i].Timestamp, (double)Quotes[i].Close);
        }

        // assert
        actual.Should().HaveCount(expected.Count);
        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void AlmaListWithDifferentParameters()
    {
        // test various parameter combinations
        var parameters = new[]
        {
            (lookback: 5, offset: 0.85, sigma: 6),
            (lookback: 10, offset: 0.5, sigma: 4),
            (lookback: 14, offset: 0.9, sigma: 8),
            (lookback: 20, offset: 0.25, sigma: 3),
            (lookback: 30, offset: 0.75, sigma: 10)
        };

        foreach (var (lookback, offset, sigma) in parameters)
        {
            // arrange
            IReadOnlyList<AlmaResult> expected = Quotes.ToAlma(lookback, offset, sigma);

            // act
            AlmaList actual = new(lookback, offset, sigma) {
                Quotes
            };

            // assert
            actual.Should().HaveCount(expected.Count,
                $"Count mismatch for parameters: lookback={lookback}, offset={offset}, sigma={sigma}");

            for (int i = 0; i < actual.Count; i++)
            {
                AlmaResult e = expected[i];
                AlmaResult a = actual[i];

                a.Timestamp.Should().Be(e.Timestamp,
                    $"Timestamp mismatch at index {i} for parameters: lookback={lookback}, offset={offset}, sigma={sigma}");

                if (e.Alma is null)
                {
                    a.Alma.Should().BeNull(
                        $"Expected null ALMA at index {i} for parameters: lookback={lookback}, offset={offset}, sigma={sigma}");
                }
                else
                {
                    a.Alma.Should().Be(e.Alma.Value,
                        $"ALMA value mismatch at index {i} for parameters: lookback={lookback}, offset={offset}, sigma={sigma}. " +
                        $"Expected: {e.Alma:F8}, Actual: {a.Alma:F8}");
                }
            }
        }
    }

    [TestMethod]
    public void AlmaListEdgeCases()
    {
        // Test with minimal lookback period (2)
        AlmaList almaList = new(2, 0.5, 2) {
            // Add first quote - should be null (not enough data)
            Quotes[0]
        };
        almaList.Should().HaveCount(1);
        almaList[0].Alma.Should().BeNull("First quote should have null ALMA");

        // Add second quote - ALMA should be available now
        almaList.Add(Quotes[1]);
        almaList.Should().HaveCount(2);

        // Compare with static series to see what's expected
        IReadOnlyList<AlmaResult> expectedResults = Quotes.Take(2).ToList().ToAlma(2, 0.5, 2);
        if (expectedResults[1].Alma is null)
        {
            almaList[1].Alma.Should().BeNull("Second quote should have null ALMA based on static series");
        }
        else
        {
            almaList[1].Alma.Should().Be(
                expectedResults[1].Alma!.Value,
                "Second quote ALMA mismatch based on static series");
        }
    }

    [TestMethod]
    public void AlmaListExceptions()
    {
        // test constructor validation
        Action act1 = () => _ = new AlmaList(1, 0.85, 6);
        act1.Should().Throw<ArgumentOutOfRangeException>("Lookback periods must be greater than 1");

        Action act2 = () => _ = new AlmaList(0, 0.85, 6);
        act2.Should().Throw<ArgumentOutOfRangeException>("Lookback periods must be greater than 1");

        Action act3 = () => _ = new AlmaList(-1, 0.85, 6);
        act3.Should().Throw<ArgumentOutOfRangeException>("Lookback periods must be greater than 1");

        Action act4 = () => _ = new AlmaList(10, 1.1, 6);
        act4.Should().Throw<ArgumentOutOfRangeException>("Offset must be between 0 and 1");

        Action act5 = () => _ = new AlmaList(10, -0.1, 6);
        act5.Should().Throw<ArgumentOutOfRangeException>("Offset must be between 0 and 1");

        Action act6 = () => _ = new AlmaList(10, 0.85, 0);
        act6.Should().Throw<ArgumentOutOfRangeException>("Sigma must be greater than 0");

        Action act7 = () => _ = new AlmaList(10, 0.85, -1);
        act7.Should().Throw<ArgumentOutOfRangeException>("Sigma must be greater than 0");

        // test null arguments
        AlmaList validList = new(10, 0.85, 6);
        Action act8 = () => validList.Add((IQuote)null!);
        act8.Should().Throw<ArgumentNullException>();

        Action act9 = () => validList.Add((IReusable)null!);
        act9.Should().Throw<ArgumentNullException>();

        Action act10 = () => validList.Add((IReadOnlyList<IQuote>)null!);
        act10.Should().Throw<ArgumentNullException>();

        Action act11 = () => validList.Add((IReadOnlyList<IReusable>)null!);
        act11.Should().Throw<ArgumentNullException>();
    }
}
