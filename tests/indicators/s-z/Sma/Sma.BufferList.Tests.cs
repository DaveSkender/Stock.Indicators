namespace BufferLists;

[TestClass]
public class SmaStreaming : BufferListTestBase
{
    [TestMethod]
    public override void FromQuote()
    {
        // arrange
        int lookbackPeriods = 14;
        IReadOnlyList<SmaResult> expected = Quotes.ToSma(lookbackPeriods);

        // act - add quotes one by one
        SmaList actual = new(lookbackPeriods);
        for (int i = 0; i < Quotes.Count; i++)
        {
            actual.Add(Quotes[i]);
        }

        // assert
        Assert.HasCount(expected.Count, actual);

        for (int i = 0; i < actual.Count; i++)
        {
            SmaResult e = expected[i];
            SmaResult a = actual[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);
            if (e.Sma is null)
            {
                Assert.IsNull(a.Sma);
            }
            else
            {
                Assert.AreEqual(e.Sma.Round(8), a.Sma.Round(8));
            }
        }
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        // arrange
        int lookbackPeriods = 20;
        IReadOnlyList<SmaResult> expected = Quotes.ToSma(lookbackPeriods);

        // act - using batch add
        SmaList actual = new(lookbackPeriods) {
            Quotes
        };

        // assert
        Assert.HasCount(expected.Count, actual);

        for (int i = 0; i < actual.Count; i++)
        {
            SmaResult e = expected[i];
            SmaResult a = actual[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);
            if (e.Sma is null)
            {
                Assert.IsNull(a.Sma);
            }
            else
            {
                Assert.AreEqual(e.Sma.Round(8), a.Sma.Round(8));
            }
        }
    }

    [TestMethod]
    public void SmaListViaReusable()
    {
        // arrange
        int lookbackPeriods = 15;
        IReadOnlyList<SmaResult> expected = Quotes.ToSma(lookbackPeriods);

        // act - using IReusable interface
        SmaList actual = new(lookbackPeriods) {
            Quotes.Use(CandlePart.Close)
        };

        // assert
        Assert.HasCount(expected.Count, actual);

        for (int i = 0; i < actual.Count; i++)
        {
            SmaResult e = expected[i];
            SmaResult a = actual[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);
            if (e.Sma is null)
            {
                Assert.IsNull(a.Sma);
            }
            else
            {
                Assert.AreEqual(e.Sma.Round(8), a.Sma.Round(8));
            }
        }
    }

    [TestMethod]
    public void SmaListViaTimestampValue()
    {
        // arrange
        int lookbackPeriods = 10;
        IReadOnlyList<SmaResult> expected = Quotes.ToSma(lookbackPeriods);

        // act - using timestamp/value method
        SmaList actual = new(lookbackPeriods);
        for (int i = 0; i < Quotes.Count; i++)
        {
            actual.Add(Quotes[i].Timestamp, (double)Quotes[i].Close);
        }

        // assert
        Assert.HasCount(expected.Count, actual);

        for (int i = 0; i < actual.Count; i++)
        {
            SmaResult e = expected[i];
            SmaResult a = actual[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);
            if (e.Sma is null)
            {
                Assert.IsNull(a.Sma);
            }
            else
            {
                Assert.AreEqual(e.Sma.Round(8), a.Sma.Round(8));
            }
        }
    }

    [TestMethod]
    public void SmaListWithDifferentPeriods()
    {
        // test various lookback periods
        int[] periods = [5, 10, 14, 20, 30];

        foreach (int period in periods)
        {
            // arrange
            IReadOnlyList<SmaResult> expected = Quotes.ToSma(period);

            // act
            SmaList actual = new(period) {
                Quotes
            };

            // assert
            Assert.HasCount(expected.Count, actual, $"Count mismatch for period {period}");

            for (int i = 0; i < actual.Count; i++)
            {
                SmaResult e = expected[i];
                SmaResult a = actual[i];

                Assert.AreEqual(e.Timestamp, a.Timestamp, $"Timestamp mismatch at index {i} for period {period}");

                if (e.Sma is null)
                {
                    Assert.IsNull(a.Sma, $"Expected null SMA at index {i} for period {period}");
                }
                else
                {
                    Assert.AreEqual(e.Sma.Round(8), a.Sma.Round(8),
                        $"SMA value mismatch at index {i} for period {period}. Expected: {e.Sma:F8}, Actual: {a.Sma:F8}");
                }
            }
        }
    }

    [TestMethod]
    public void SmaListEdgeCases()
    {
        // Test with minimal lookback period (1)
        SmaList smaList = new(1) {
            // Add first quote - should have SMA immediately
            Quotes[0]
        };
        Assert.HasCount(1, smaList);
        Assert.AreEqual((double)Quotes[0].Close, smaList[0].Sma, "First quote should have SMA equal to close price for period 1");

        // Add second quote - should replace the first
        smaList.Add(Quotes[1]);
        Assert.HasCount(2, smaList);
        Assert.AreEqual((double)Quotes[1].Close, smaList[1].Sma, "Second quote should have SMA equal to close price for period 1");

        // Test with period 2
        SmaList smaList2 = new(2) {
            Quotes[0]
        };
        Assert.HasCount(1, smaList2);
        Assert.IsNull(smaList2[0].Sma, "First quote should have null SMA for period 2");

        smaList2.Add(Quotes[1]);
        Assert.HasCount(2, smaList2);
        double expectedSma = ((double)Quotes[0].Close + (double)Quotes[1].Close) / 2;
        Assert.AreEqual(expectedSma, smaList2[1].Sma, "Second quote should have correct SMA for period 2");
    }

    [TestMethod]
    public void ClearResetsState()
    {
        int lookbackPeriods = 14;
        List<Quote> subset = Quotes.Take(120).ToList();

        SmaList smaList = new(lookbackPeriods);

        for (int i = 0; i < subset.Count; i++)
        {
            smaList.Add(subset[i]);
        }

        Assert.IsTrue(smaList.Any(r => r.Sma.HasValue), "Warm-up should complete before clearing");

        smaList.Clear();

        Assert.IsEmpty(smaList, "Clear should remove existing results");

        for (int i = 0; i < subset.Count; i++)
        {
            smaList.Add(subset[i]);
        }

        IReadOnlyList<SmaResult> expected = subset.ToSma(lookbackPeriods);

        Assert.AreEqual(expected.Count, smaList.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            SmaResult expectedResult = expected[i];
            SmaResult actualResult = smaList[i];

            Assert.AreEqual(expectedResult.Timestamp, actualResult.Timestamp);
            if (expectedResult.Sma is null)
            {
                Assert.IsNull(actualResult.Sma);
            }
            else
            {
                Assert.AreEqual(expectedResult.Sma.Round(8), actualResult.Sma.Round(8));
            }
        }
    }

    [TestMethod]
    public void SmaListExceptions()
    {
        // test constructor validation
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new SmaList(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new SmaList(-1));

        // test null arguments
        SmaList validList = new(10);
        Assert.ThrowsExactly<ArgumentNullException>(() => validList.Add((IQuote)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => validList.Add((IReusable)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => validList.Add((IReadOnlyList<IQuote>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => validList.Add((IReadOnlyList<IReusable>)null!));
    }
}
