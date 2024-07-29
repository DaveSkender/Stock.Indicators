namespace StaticSeries;

[TestClass]
public class ParabolicSarTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        double acclerationStep = 0.02;
        double maxAccelerationFactor = 0.2;

        IReadOnlyList<ParabolicSarResult> results =
            Quotes.GetParabolicSar(acclerationStep, maxAccelerationFactor)
                .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Sar != null));

        // sample values
        ParabolicSarResult r14 = results[14];
        Assert.AreEqual(212.83, r14.Sar);
        Assert.AreEqual(true, r14.IsReversal);

        ParabolicSarResult r16 = results[16];
        Assert.AreEqual(212.9924, r16.Sar.Round(4));
        Assert.AreEqual(false, r16.IsReversal);

        ParabolicSarResult r94 = results[94];
        Assert.AreEqual(228.3600, r94.Sar.Round(4));
        Assert.AreEqual(false, r94.IsReversal);

        ParabolicSarResult r501 = results[501];
        Assert.AreEqual(229.7662, r501.Sar.Round(4));
        Assert.AreEqual(false, r501.IsReversal);
    }

    [TestMethod]
    public void Extended()
    {
        double acclerationStep = 0.02;
        double maxAccelerationFactor = 0.2;
        double initialStep = 0.01;

        IReadOnlyList<ParabolicSarResult> results =
            Quotes.GetParabolicSar(
                acclerationStep, maxAccelerationFactor, initialStep)
                .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Sar != null));

        // sample values
        ParabolicSarResult r14 = results[14];
        Assert.AreEqual(212.83, r14.Sar);
        Assert.AreEqual(true, r14.IsReversal);

        ParabolicSarResult r16 = results[16];
        Assert.AreEqual(212.9518, r16.Sar.Round(4));
        Assert.AreEqual(false, r16.IsReversal);

        ParabolicSarResult r94 = results[94];
        Assert.AreEqual(228.36, r94.Sar);
        Assert.AreEqual(false, r94.IsReversal);

        ParabolicSarResult r486 = results[486];
        Assert.AreEqual(273.4148, r486.Sar.Round(4));
        Assert.AreEqual(false, r486.IsReversal);

        ParabolicSarResult r501 = results[501];
        Assert.AreEqual(246.73, r501.Sar);
        Assert.AreEqual(false, r501.IsReversal);
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetParabolicSar()
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(479, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void InsufficientQuotes()
    {
        double acclerationStep = 0.02;
        double maxAccelerationFactor = 0.2;

        IEnumerable<Quote> insufficientQuotes = Data.GetDefault()
            .OrderBy(x => x.Timestamp)
            .Take(10);

        IReadOnlyList<ParabolicSarResult> results =
            insufficientQuotes.GetParabolicSar(acclerationStep, maxAccelerationFactor)
                .ToList();

        // assertions

        // proper quantities
        Assert.AreEqual(10, results.Count);
        Assert.AreEqual(0, results.Count(x => x.Sar != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ParabolicSarResult> r = BadQuotes
            .GetParabolicSar(0.2, 0.2, 0.2);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Sar is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ParabolicSarResult> r0 = Noquotes
            .GetParabolicSar();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<ParabolicSarResult> r1 = Onequote
            .GetParabolicSar();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        double acclerationStep = 0.02;
        double maxAccelerationFactor = 0.2;

        IReadOnlyList<ParabolicSarResult> results = Quotes
            .GetParabolicSar(acclerationStep, maxAccelerationFactor)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(488, results.Count);

        ParabolicSarResult last = results[^1];
        Assert.AreEqual(229.7662, last.Sar.Round(4));
        Assert.AreEqual(false, last.IsReversal);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad acceleration step
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetParabolicSar(0, 1));

        // insufficient acceleration step
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetParabolicSar(0.02, 0));

        // step larger than factor
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetParabolicSar(6, 2));

        // insufficient initial factor
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetParabolicSar(0.02, 0.5, 0));
    }
}
