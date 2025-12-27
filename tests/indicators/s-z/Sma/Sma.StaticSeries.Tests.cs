namespace StaticSeries;

[TestClass]
public partial class SmaTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToSma(20);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(483);

        // sample values
        sut[18].Sma.Should().BeNull();
        sut[19].Sma.Should().BeApproximately(214.5250, Money4);
        sut[24].Sma.Should().BeApproximately(215.0310, Money4);
        sut[149].Sma.Should().BeApproximately(234.9350, Money4);
        sut[249].Sma.Should().BeApproximately(255.5500, Money4);
        sut[501].Sma.Should().BeApproximately(251.8600, Money4);
    }

    [TestMethod]
    public void CandlePartOpen()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .Use(CandlePart.Open)
            .ToSma(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(483);

        // sample values
        sut[18].Sma.Should().BeNull();
        sut[19].Sma.Should().BeApproximately(214.3795, Money4);
        sut[24].Sma.Should().BeApproximately(214.9535, Money4);
        sut[149].Sma.Should().BeApproximately(234.8280, Money4);
        sut[249].Sma.Should().BeApproximately(255.6915, Money4);
        sut[501].Sma.Should().BeApproximately(253.1725, Money4);
    }

    [TestMethod]
    public void CandlePartVolume()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .Use(CandlePart.Volume)
            .ToSma(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(483);

        // sample values
        SmaResult r24 = sut[24];
        r24.Sma.Should().Be(77293768.2);

        SmaResult r290 = sut[290];
        r290.Sma.Should().Be(157958070.8);

        SmaResult r501 = sut[501];
        Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", invariantCulture), r501.Timestamp);
        r501.Sma.Should().Be(163695200);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<EmaResult> sut = Quotes
            .ToSma(10)
            .ToEma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Ema != null).Should().HaveCount(484);
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<SmaResult> r = Data.GetBtcUsdNan()
            .ToSma(50);

        r.Where(static x => x.Sma is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<SmaResult> r = BadQuotes
            .ToSma(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Sma is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<SmaResult> r0 = Noquotes
            .ToSma(5);

        r0.Should().BeEmpty();

        IReadOnlyList<SmaResult> r1 = Onequote
            .ToSma(5);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToSma(20)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 19);
        sut[^1].Sma.Should().BeApproximately(251.8600, Money4);
    }

    [TestMethod]
    public void Equality()
    {
        SmaResult r1 = new(Timestamp: EvalDate, Sma: 1d);

        SmaResult r2 = new(Timestamp: EvalDate, Sma: 1d);

        SmaResult r3 = new(Timestamp: EvalDate, Sma: 2d);

        Assert.IsTrue(Equals(r1, r2));
        Assert.IsFalse(Equals(r1, r3));

        Assert.IsTrue(r1.Equals(r2));
        Assert.IsFalse(r1.Equals(r3));

        (r1 == r2).Should().BeTrue();
        (r1 == r3).Should().BeFalse();

        (r1 != r2).Should().BeFalse();
        (r1 != r3).Should().BeTrue();
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToSma(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();

    /// <summary>
    /// Array-based interface tests
    /// </summary>
    [TestMethod]
    public void ArrayInterface_DefaultParameters()
    {
        // convert quotes to array
        double[] values = Quotes.Select(static q => (double)q.Close).ToArray();

        // calculate using array method
        double[] sut = values.ToSma(20);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Count(static x => !double.IsNaN(x)).Should().Be(483);

        // sample values - should match the IReusable interface results
        double.IsNaN(sut[18]).Should().BeTrue();
        sut[19].Should().BeApproximately(214.5250, Money4);
        sut[24].Should().BeApproximately(215.0310, Money4);
        sut[149].Should().BeApproximately(234.9350, Money4);
        sut[249].Should().BeApproximately(255.5500, Money4);
        sut[501].Should().BeApproximately(251.8600, Money4);
    }

    [TestMethod]
    public void ArrayInterface_Parity()
    {
        // convert quotes to array
        double[] values = Quotes.Select(static q => (double)q.Close).ToArray();

        // calculate using both methods
        double[] arrayResults = values.ToSma(10);
        IReadOnlyList<SmaResult> objectResults = Quotes.ToSma(10);

        // ensure results match
        arrayResults.Should().HaveCount(objectResults.Count);

        for (int i = 0; i < arrayResults.Length; i++)
        {
            if (double.IsNaN(arrayResults[i]))
            {
                objectResults[i].Sma.Should().BeNull();
            }
            else
            {
                objectResults[i].Sma.Should().BeApproximately(arrayResults[i], Money6);
            }
        }
    }

    [TestMethod]
    public void ArrayInterface_EmptyArray()
    {
        double[] empty = [];
        double[] sut = empty.ToSma(5);
        sut.Should().BeEmpty();
    }

    [TestMethod]
    public void ArrayInterface_Exceptions()
    {
        double[] values = Quotes.Select(static q => (double)q.Close).ToArray();

        // null array
        FluentActions
            .Invoking(static () => ((double[])null!).ToSma(10))
            .Should()
            .ThrowExactly<ArgumentNullException>();

        // invalid lookback
        FluentActions
            .Invoking(() => values.ToSma(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Precision drift comparison between brute force, rolling sum, and SIMD methods.
    /// Uses the longest available dataset (15,821 S and P 500 daily bars) to demonstrate
    /// floating-point accumulation errors over time in different optimization approaches.
    /// Uses decimal calculation as the precision baseline for comparison.
    /// </summary>
    [TestMethod]
    public void PrecisionComparison_RollingSumVsBruteForce()
    {
        // use longest dataset (~62 years of S&P 500 daily data, 15,821 bars)
        double[] values = LongestQuotes.ToValueArray();
        decimal[] decimalValues = LongestQuotes.Select(q => q.Close).ToArray();
        const int lookbackPeriods = 20;
        int length = values.Length;
        List<PrecisionRecord> results = new(LongestQuotes.Count);

        // allocate result arrays
        double[] fullResults = new double[length];
        double[] loopResults = new double[length];
        double[] rollResults = new double[length];
        double[] simdResults = new double[length];

        double rollingSum = double.NaN;

        // calculate all methods
        for (int i = 0; i < length; i++)
        {
            // decimal baseline (highest precision)
            if (i >= lookbackPeriods - 1)
            {
                decimal sum = 0m;
                for (int j = i - lookbackPeriods + 1; j <= i; j++)
                {
                    sum += decimalValues[j];
                }

                fullResults[i] = (double)(sum / lookbackPeriods);
            }
            else
            {
                fullResults[i] = double.NaN;
            }

            // double-based methods
            loopResults[i] = Sma.Increment(
                source: values,
                lookbackPeriods: lookbackPeriods,
                endIndex: i);

            (double average, double newSum, double dropValue) = Sma.Increment(
                source: values,
                lookbackPeriods: lookbackPeriods,
                endIndex: i,
                priorSum: rollingSum);

            rollingSum = newSum;
            rollResults[i] = average;

            simdResults[i] = Sma.IncrementSimd(
                source: values,
                lookbackPeriods: lookbackPeriods,
                endIndex: i);

            // record precision differences compared to decimal baseline
            results.Add(new PrecisionRecord(
                Timestamp: LongestQuotes[i].Timestamp,
                Index: i,
                Full: fullResults[i],
                Loop: loopResults[i],
                Roll: rollResults[i],
                Simd: simdResults[i],
                LoopDiff: loopResults[i] - fullResults[i],
                RollDiff: rollResults[i] - fullResults[i],
                SimdDiff: simdResults[i] - fullResults[i]));
        }

        results.TakeLast(50).ToList().ToConsole(("LoopDiff", "F16"), ("RollDiff", "F16"), ("SimdDiff", "F16"));

        // Analyze precision differences
        List<PrecisionRecord> warmedUp = results.Skip(lookbackPeriods).ToList();

        double maxLoopDiff = warmedUp.Max(r => Math.Abs(r.LoopDiff));
        double maxRollDiff = warmedUp.Max(r => Math.Abs(r.RollDiff));
        double maxSimdDiff = warmedUp.Max(r => Math.Abs(r.SimdDiff));

        double avgLoopDiff = warmedUp.Average(r => Math.Abs(r.LoopDiff));
        double avgRollDiff = warmedUp.Average(r => Math.Abs(r.RollDiff));
        double avgSimdDiff = warmedUp.Average(r => Math.Abs(r.SimdDiff));

        // Get sample data points for inspection
        PrecisionRecord last = results[^1];
        PrecisionRecord middle = results[length / 2];
        PrecisionRecord afterWarmup = results[lookbackPeriods + 100];

        // Build diagnostic message
        string diagnostics = "\n" +
            "Precision Analysis (vs Decimal Baseline):\n" +
            $"{'=',-70}\n" +
            "Method   Max Abs Error    Avg Abs Error\n" +
            $"{'-',-70}\n" +
            $"Loop     {maxLoopDiff,-16:E9}  {avgLoopDiff,-16:E9}\n" +
            $"Roll     {maxRollDiff,-16:E9}  {avgRollDiff,-16:E9}\n" +
            $"Simd     {maxSimdDiff,-16:E9}  {avgSimdDiff,-16:E9}\n\n" +
            "Sample Points:\n" +
            $"{'-',-70}\n" +
            $"After Warmup (index {afterWarmup.Index}):\n" +
            $"  Loop: {afterWarmup.LoopDiff,18:E9}  Roll: {afterWarmup.RollDiff,18:E9}  Simd: {afterWarmup.SimdDiff,18:E9}\n" +
            $"Middle (index {middle.Index}):\n" +
            $"  Loop: {middle.LoopDiff,18:E9}  Roll: {middle.RollDiff,18:E9}  Simd: {middle.SimdDiff,18:E9}\n" +
            $"Last (index {last.Index}):\n" +
            $"  Loop: {last.LoopDiff,18:E9}  Roll: {last.RollDiff,18:E9}  Simd: {last.SimdDiff,18:E9}\n";

        Assert.Inconclusive(diagnostics);
    }

    private record PrecisionRecord(
        DateTime Timestamp,
        int Index,
        double Full,
        double Loop,
        double Roll,
        double Simd,
        double LoopDiff,
        double RollDiff,
        double SimdDiff) : ISeries;
}
