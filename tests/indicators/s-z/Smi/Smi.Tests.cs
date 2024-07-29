namespace StaticSeries;

[TestClass]
public class SmiTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<SmiResult> results = Quotes
            .GetSmi(14, 20, 5);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.Smi != null));
        Assert.AreEqual(489, results.Count(x => x.Signal != null));

        // sample values
        SmiResult r12 = results[12];
        Assert.IsNull(r12.Smi);
        Assert.IsNull(r12.Signal);

        SmiResult r13 = results[13];
        Assert.AreEqual(17.2603, r13.Smi.Round(4));
        Assert.AreEqual(17.2603, r13.Signal.Round(4));

        SmiResult r14 = results[14];
        Assert.AreEqual(18.6086, r14.Smi.Round(4));
        Assert.AreEqual(17.9344, r14.Signal.Round(4));

        SmiResult r28 = results[28];
        Assert.AreEqual(51.0417, r28.Smi.Round(4));
        Assert.AreEqual(47.1207, r28.Signal.Round(4));

        SmiResult r150 = results[150];
        Assert.AreEqual(65.6692, r150.Smi.Round(4));
        Assert.AreEqual(66.3292, r150.Signal.Round(4));

        SmiResult r250 = results[250];  // also testing aliases here
        Assert.AreEqual(67.2534, r250.Smi.Round(4));
        Assert.AreEqual(67.6261, r250.Signal.Round(4));

        SmiResult r501 = results[501];
        Assert.AreEqual(-52.6560, r501.Smi.Round(4));
        Assert.AreEqual(-54.1903, r501.Signal.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetSmi(14, 20, 5)
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NoSignal()
    {
        IReadOnlyList<SmiResult> results = Quotes
            .GetSmi(5, 20, 20, 1);

        // signal equals oscillator
        SmiResult r1 = results[487];
        Assert.AreEqual(r1.Smi, r1.Signal);

        SmiResult r2 = results[501];
        Assert.AreEqual(r2.Smi, r2.Signal);
    }

    [TestMethod]
    public void SmallPeriods()
    {
        IReadOnlyList<SmiResult> results = Quotes
            .GetSmi(1, 1, 1, 5);

        // sample values
        SmiResult r51 = results[51];
        Assert.AreEqual(-100, r51.Smi.Round(4));
        Assert.AreEqual(-20.8709, r51.Signal.Round(4));

        SmiResult r81 = results[81];
        Assert.AreEqual(0, r81.Smi.Round(4));
        Assert.AreEqual(-14.7101, r81.Signal.Round(4));

        SmiResult r88 = results[88];
        Assert.AreEqual(100, r88.Smi.Round(4));
        Assert.AreEqual(47.2291, r88.Signal.Round(4));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<SmiResult> r = BadQuotes
            .GetSmi(5, 5, 1, 5);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Smi is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<SmiResult> r0 = Noquotes
            .GetSmi(5, 5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<SmiResult> r1 = Onequote
            .GetSmi(5, 3, 3);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmiResult> results = Quotes
            .GetSmi(14, 20, 5)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(501 - (14 + 100), results.Count);

        SmiResult last = results[^1];
        Assert.AreEqual(-52.6560, last.Smi.Round(4));
        Assert.AreEqual(-54.1903, last.Signal.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetSmi(0, 5, 5, 5));

        // bad first smooth period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetSmi(14, 0, 5, 5));

        // bad second smooth period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetSmi(14, 3, 0, 5));

        // bad signal
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetSmi(9, 3, 1, 0));
    }
}
