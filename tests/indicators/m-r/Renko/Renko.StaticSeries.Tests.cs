namespace StaticSeries;

[TestClass]
public class Renko : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()  // close
    {
        IReadOnlyList<RenkoResult> results = Quotes
            .GetRenko(2.5m);

        // assertions

        Assert.AreEqual(112, results.Count);
        Assert.AreEqual(62, results.Count(x => x.IsUp));
        Assert.AreEqual(50, results.Count(x => !x.IsUp));

        // sample values
        RenkoResult r0 = results[0];
        Assert.AreEqual(213m, r0.Open);
        Assert.AreEqual(216.89m, r0.High);
        Assert.AreEqual(212.53m, r0.Low);
        Assert.AreEqual(215.5m, r0.Close);
        Assert.AreEqual(1180981564m, r0.Volume);
        Assert.IsTrue(r0.IsUp);

        RenkoResult r5 = results[5];
        Assert.AreEqual(225.5m, r5.Open);
        Assert.AreEqual(228.15m, r5.High);
        Assert.AreEqual(219.77m, r5.Low);
        Assert.AreEqual(228m, r5.Close);
        Assert.AreEqual(4192959240m, r5.Volume);
        Assert.IsTrue(r5.IsUp);

        RenkoResult last = results[^1];
        Assert.AreEqual(240.5m, last.Open);
        Assert.AreEqual(243.68m, last.High);
        Assert.AreEqual(234.52m, last.Low);
        Assert.AreEqual(243m, last.Close);
        Assert.AreEqual(189794032m, last.Volume);
        Assert.IsTrue(last.IsUp);
    }

    [TestMethod]
    public void StandardHighLow()
    {
        IReadOnlyList<RenkoResult> results = Quotes
            .GetRenko(2.5m, EndType.HighLow);

        // assertions

        Assert.AreEqual(159, results.Count);

        // sample values
        RenkoResult r0 = results[0];
        Assert.AreEqual(213m, r0.Open);
        Assert.AreEqual(216.89m, r0.High);
        Assert.AreEqual(212.53m, r0.Low);
        Assert.AreEqual(215.5m, r0.Close);
        Assert.AreEqual(1180981564m, r0.Volume.Round(0));
        Assert.IsTrue(r0.IsUp);

        RenkoResult r25 = results[25];
        Assert.AreEqual(270.5m, r25.Open);
        Assert.AreEqual(273.16m, r25.High);
        Assert.AreEqual(271.96m, r25.Low);
        Assert.AreEqual(273m, r25.Close);
        Assert.AreEqual(100801672m, r25.Volume.Round(0));
        Assert.IsTrue(r25.IsUp);

        RenkoResult last = results[^1];
        Assert.AreEqual(243m, last.Open);
        Assert.AreEqual(246.73m, last.High);
        Assert.AreEqual(241.87m, last.Low);
        Assert.AreEqual(245.5m, last.Close);
        Assert.AreEqual(51999637m, last.Volume.Round(0));
        Assert.IsTrue(last.IsUp);
    }

    [TestMethod]
    public void Atr()
    {
        IReadOnlyList<RenkoResult> results = Quotes
            .GetRenkoAtr(14);

        // proper quantities
        Assert.AreEqual(29, results.Count);

        // sample values
        RenkoResult r0 = results[0];
        Assert.AreEqual(212.8m, r0.Open.Round(4));
        Assert.AreEqual(220.19m, r0.High.Round(4));
        Assert.AreEqual(212.53m, r0.Low.Round(4));
        Assert.AreEqual(218.9497m, r0.Close.Round(4));
        Assert.AreEqual(2090292272m, r0.Volume.Round(0));
        Assert.IsTrue(r0.IsUp);

        RenkoResult last = results[^1];
        Assert.AreEqual(237.3990m, last.Open.Round(4));
        Assert.AreEqual(246.73m, last.High.Round(4));
        Assert.AreEqual(229.42m, last.Low.Round(4));
        Assert.AreEqual(243.5487m, last.Close.Round(4));
        Assert.AreEqual(715446448m, last.Volume.Round(0));
        Assert.IsTrue(last.IsUp);
    }

    [TestMethod]
    public void UseAsQuotes()
    {
        IReadOnlyList<RenkoResult> renkoQuotes = Quotes.GetRenko(2.5m);
        IReadOnlyList<SmaResult> renkoSma = renkoQuotes.ToSma(5);
        Assert.AreEqual(108, renkoSma.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<RenkoResult> r = BadQuotes
            .GetRenko(100m);

        Assert.AreNotEqual(0, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<RenkoResult> r0 = Noquotes
            .GetRenko(0.01m);

        Assert.AreEqual(0, r0.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad arguments
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetRenko(0));

        // bad end type
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetRenko(2, (EndType)int.MaxValue));
    }
}
