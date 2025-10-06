namespace BufferLists;

[TestClass]
public class BollingerBandsBufferList : BufferListTestBase
{
    private const int lookbackPeriods = 20;
    private const double standardDeviations = 2;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<BollingerBandsResult> series
       = Quotes.ToBollingerBands(lookbackPeriods, standardDeviations);

    [TestMethod]
    public void FromReusableSplit()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

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
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations) { Quotes };

        IReadOnlyList<BollingerBandsResult> series
            = Quotes.ToBollingerBands(lookbackPeriods, standardDeviations);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        BollingerBandsList sut = new(lookbackPeriods, standardDeviations, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<BollingerBandsResult> expected = subset.ToBollingerBands(lookbackPeriods, standardDeviations);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void BollingerBandsListWithDifferentPeriods()
    {
        // test various lookback periods
        int[] periods = [5, 10, 14, 20, 30];

        foreach (int period in periods)
        {
            // arrange
            IReadOnlyList<BollingerBandsResult> expected = Quotes.ToBollingerBands(period, standardDeviations);

            // act
            BollingerBandsList actual = new(period, standardDeviations) {
                Quotes
            };

            // assert
            actual.Should().HaveCount(expected.Count, $"Count mismatch for period {period}");

            for (int i = 0; i < actual.Count; i++)
            {
                BollingerBandsResult e = expected[i];
                BollingerBandsResult a = actual[i];

                a.Timestamp.Should().Be(e.Timestamp, $"Timestamp mismatch at index {i} for period {period}");

                if (e.Sma is null)
                {
                    a.Sma.Should().BeNull($"SMA should be null at index {i} for period {period}");
                }
                else
                {
                    a.Sma.Should().Be(e.Sma.Value, $"SMA mismatch at index {i} for period {period}");
                }

                if (e.UpperBand is null)
                {
                    a.UpperBand.Should().BeNull($"UpperBand should be null at index {i} for period {period}");
                }
                else
                {
                    a.UpperBand.Should().Be(e.UpperBand.Value, $"UpperBand mismatch at index {i} for period {period}");
                }

                if (e.LowerBand is null)
                {
                    a.LowerBand.Should().BeNull($"LowerBand should be null at index {i} for period {period}");
                }
                else
                {
                    a.LowerBand.Should().Be(e.LowerBand.Value, $"LowerBand mismatch at index {i} for period {period}");
                }
            }
        }
    }

    [TestMethod]
    public void BollingerBandsListEdgeCases()
    {
        // Test with minimal lookback period (2)
        BollingerBandsList bbList = new(2, standardDeviations) {
            // Add first quote - should be null (not enough data)
            Quotes[0]
        };
        bbList.Should().HaveCount(1);
        bbList[0].Sma.Should().BeNull("First quote should have null SMA");

        // Add second quote - BB might be available (need to check against static series)
        bbList.Add(Quotes[1]);
        bbList.Should().HaveCount(2);

        // Compare with static series to see what's expected
        IReadOnlyList<BollingerBandsResult> expectedResults = Quotes.Take(2).ToList().ToBollingerBands(2, standardDeviations);
        if (expectedResults[1].Sma is null)
        {
            bbList[1].Sma.Should().BeNull("Second quote should have null SMA when period is 2");
        }
        else
        {
            bbList[1].Sma.Should().Be(expectedResults[1].Sma.Value, "Second quote SMA should match expected");
        }
    }

    [TestMethod]
    public void BollingerBandsListExceptions()
    {
        // Test invalid constructor parameters
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BollingerBandsList(1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BollingerBandsList(20, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BollingerBandsList(20, -1));

        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

        // Test null arguments
        Assert.ThrowsExactly<ArgumentNullException>(() => sut.Add((IReusable)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => sut.Add((IQuote)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => sut.Add((IReadOnlyList<IReusable>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => sut.Add((IReadOnlyList<IQuote>)null!));
    }
}
