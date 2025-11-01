namespace StaticSeries;

[TestClass]
public class ParabolicSar : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        const double acclerationStep = 0.02;
        const double maxAccelerationFactor = 0.2;

        List<ParabolicSarResult> results =
            Quotes.ToParabolicSar(acclerationStep, maxAccelerationFactor)
                .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Sar != null));

        // sample values
        ParabolicSarResult r14 = results[14];
        Assert.AreEqual(212.83, r14.Sar);
        Assert.IsTrue(r14.IsReversal);

        ParabolicSarResult r16 = results[16];
        Assert.AreEqual(212.9924, r16.Sar.Round(4));
        Assert.IsFalse(r16.IsReversal);

        ParabolicSarResult r94 = results[94];
        Assert.AreEqual(228.3600, r94.Sar.Round(4));
        Assert.IsFalse(r94.IsReversal);

        ParabolicSarResult r501 = results[501];
        Assert.AreEqual(229.7662, r501.Sar.Round(4));
        Assert.IsFalse(r501.IsReversal);
    }

    [TestMethod]
    public void Extended()
    {
        const double acclerationStep = 0.02;
        const double maxAccelerationFactor = 0.2;
        const double initialStep = 0.01;

        List<ParabolicSarResult> results =
            Quotes.ToParabolicSar(
                acclerationStep, maxAccelerationFactor, initialStep)
                .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Sar != null));

        // sample values
        ParabolicSarResult r14 = results[14];
        Assert.AreEqual(212.83, r14.Sar);
        Assert.IsTrue(r14.IsReversal);

        ParabolicSarResult r16 = results[16];
        Assert.AreEqual(212.9518, r16.Sar.Round(4));
        Assert.IsFalse(r16.IsReversal);

        ParabolicSarResult r94 = results[94];
        Assert.AreEqual(228.36, r94.Sar);
        Assert.IsFalse(r94.IsReversal);

        ParabolicSarResult r486 = results[486];
        Assert.AreEqual(273.4148, r486.Sar.Round(4));
        Assert.IsFalse(r486.IsReversal);

        ParabolicSarResult r501 = results[501];
        Assert.AreEqual(246.73, r501.Sar);
        Assert.IsFalse(r501.IsReversal);
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToParabolicSar()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(479, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void InsufficientQuotes()
    {
        const double acclerationStep = 0.02;
        const double maxAccelerationFactor = 0.2;

        List<Quote> insufficientQuotes = Data.GetDefault()
            .OrderBy(static x => x.Timestamp)
            .Take(10)
            .ToList();

        IReadOnlyList<ParabolicSarResult> results =
            insufficientQuotes
                .ToParabolicSar(acclerationStep, maxAccelerationFactor);

        // assertions

        // proper quantities
        Assert.HasCount(10, results);
        Assert.IsEmpty(results.Where(static x => x.Sar != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ParabolicSarResult> r = BadQuotes
            .ToParabolicSar(0.2, 0.2, 0.2);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Sar is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ParabolicSarResult> r0 = Noquotes
            .ToParabolicSar();

        Assert.IsEmpty(r0);

        IReadOnlyList<ParabolicSarResult> r1 = Onequote
            .ToParabolicSar();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        const double acclerationStep = 0.02;
        const double maxAccelerationFactor = 0.2;

        IReadOnlyList<ParabolicSarResult> results = Quotes
            .ToParabolicSar(acclerationStep, maxAccelerationFactor)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(488, results);

        ParabolicSarResult last = results[^1];
        Assert.AreEqual(229.7662, last.Sar.Round(4));
        Assert.IsFalse(last.IsReversal);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad acceleration step
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToParabolicSar(0, 1));

        // insufficient acceleration step
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToParabolicSar(0.02, 0));

        // step larger than factor
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToParabolicSar(6, 2));

        // insufficient initial factor
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToParabolicSar(0.02, 0.5, 0));
    }
}
