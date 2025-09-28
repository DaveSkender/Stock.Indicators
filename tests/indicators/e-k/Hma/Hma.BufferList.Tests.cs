namespace BufferLists;

[TestClass]
public class HmaStreaming : BufferListTestBase
{
    [TestMethod]
    public override void FromQuote()
    {
        // arrange
        int lookbackPeriods = 14;
        IReadOnlyList<HmaResult> expected = Quotes.ToHma(lookbackPeriods);

        // act - add quotes one by one
        HmaList actual = new(lookbackPeriods);
        for (int i = 0; i < Quotes.Count; i++)
        {
            actual.Add(Quotes[i]);
        }

        // assert
        Assert.HasCount(expected.Count, actual);

        for (int i = 0; i < actual.Count; i++)
        {
            HmaResult e = expected[i];
            HmaResult a = actual[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);
            if (e.Hma is null)
            {
                Assert.IsNull(a.Hma);
            }
            else
            {
                Assert.AreEqual(e.Hma.Round(8), a.Hma.Round(8));
            }
        }
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        // arrange
        int lookbackPeriods = 20;
        IReadOnlyList<HmaResult> expected = Quotes.ToHma(lookbackPeriods);

        // act - using batch add
        HmaList actual = new(lookbackPeriods);
        actual.Add(Quotes);

        // assert
        Assert.HasCount(expected.Count, actual);

        for (int i = 0; i < actual.Count; i++)
        {
            HmaResult e = expected[i];
            HmaResult a = actual[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);
            if (e.Hma is null)
            {
                Assert.IsNull(a.Hma);
            }
            else
            {
                Assert.AreEqual(e.Hma.Round(8), a.Hma.Round(8));
            }
        }
    }

    [TestMethod]
    public void HmaListViaReusable()
    {
        // arrange
        int lookbackPeriods = 15;
        IReadOnlyList<HmaResult> expected = Quotes.ToHma(lookbackPeriods);

        // act - using IReusable interface
        HmaList actual = new(lookbackPeriods);
        actual.Add(Quotes.Use(CandlePart.Close));

        // assert
        Assert.HasCount(expected.Count, actual);

        for (int i = 0; i < actual.Count; i++)
        {
            HmaResult e = expected[i];
            HmaResult a = actual[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);
            if (e.Hma is null)
            {
                Assert.IsNull(a.Hma);
            }
            else
            {
                Assert.AreEqual(e.Hma.Round(8), a.Hma.Round(8));
            }
        }
    }

    [TestMethod]
    public void HmaListViaTimestampValue()
    {
        // arrange
        int lookbackPeriods = 10;
        IReadOnlyList<HmaResult> expected = Quotes.ToHma(lookbackPeriods);

        // act - using timestamp/value method
        HmaList actual = new(lookbackPeriods);
        for (int i = 0; i < Quotes.Count; i++)
        {
            actual.Add(Quotes[i].Timestamp, (double)Quotes[i].Close);
        }

        // assert
        Assert.HasCount(expected.Count, actual);

        for (int i = 0; i < actual.Count; i++)
        {
            HmaResult e = expected[i];
            HmaResult a = actual[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);
            if (e.Hma is null)
            {
                Assert.IsNull(a.Hma);
            }
            else
            {
                Assert.AreEqual(e.Hma.Round(8), a.Hma.Round(8));
            }
        }
    }

    [TestMethod]
    public void HmaListWithDifferentPeriods()
    {
        // test various lookback periods
        int[] periods = [5, 10, 14, 20, 30];

        foreach (int period in periods)
        {
            // arrange
            IReadOnlyList<HmaResult> expected = Quotes.ToHma(period);

            // act
            HmaList actual = new(period);
            actual.Add(Quotes);

            // assert
            Assert.HasCount(expected.Count, actual, $"Count mismatch for period {period}");

            for (int i = 0; i < actual.Count; i++)
            {
                HmaResult e = expected[i];
                HmaResult a = actual[i];

                Assert.AreEqual(e.Timestamp, a.Timestamp, $"Timestamp mismatch at index {i} for period {period}");

                if (e.Hma is null)
                {
                    Assert.IsNull(a.Hma, $"Expected null HMA at index {i} for period {period}");
                }
                else
                {
                    Assert.AreEqual(e.Hma.Round(8), a.Hma.Round(8),
                        $"HMA value mismatch at index {i} for period {period}. Expected: {e.Hma:F8}, Actual: {a.Hma:F8}");
                }
            }
        }
    }

    [TestMethod]
    public void HmaListEdgeCases()
    {
        // Test with minimal lookback period (2)
        HmaList hmaList = new(2);

        // Add first quote - should be null (not enough data)
        hmaList.Add(Quotes[0]);
        Assert.HasCount(1, hmaList);
        Assert.IsNull(hmaList[0].Hma, "First quote should have null HMA");

        // Add second quote - HMA might be available (need to check against static series)
        hmaList.Add(Quotes[1]);
        Assert.HasCount(2, hmaList);

        // Compare with static series to see what's expected  
        var expectedResults = Quotes.Take(2).ToList().ToHma(2);
        if (expectedResults[1].Hma is null)
        {
            Assert.IsNull(hmaList[1].Hma, "Second quote should have null HMA based on static series");
        }
        else
        {
            Assert.IsNotNull(hmaList[1].Hma, "Second quote should have HMA value based on static series");
        }
    }

    [TestMethod]
    public void HmaListExceptions()
    {
        // test constructor validation
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new HmaList(1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new HmaList(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new HmaList(-1));

        // test null arguments
        HmaList validList = new(10);
        Assert.ThrowsExactly<ArgumentNullException>(() => validList.Add((IQuote)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => validList.Add((IReusable)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => validList.Add((IReadOnlyList<IQuote>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => validList.Add((IReadOnlyList<IReusable>)null!));
    }
}
