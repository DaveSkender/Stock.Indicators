using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class RollingPivots : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int windowPeriods = 11;
        int offsetPeriods = 9;
        PivotPointType pointType = PivotPointType.Standard;

        List<RollingPivotsResult> results =
            quotes.GetRollingPivots(windowPeriods, offsetPeriods, pointType)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.PP != null));

        // sample values
        RollingPivotsResult r1 = results[19];
        Assert.AreEqual(null, r1.PP);
        Assert.AreEqual(null, r1.S1);
        Assert.AreEqual(null, r1.S2);
        Assert.AreEqual(null, r1.S3);
        Assert.AreEqual(null, r1.S4);
        Assert.AreEqual(null, r1.R1);
        Assert.AreEqual(null, r1.R2);
        Assert.AreEqual(null, r1.R3);
        Assert.AreEqual(null, r1.R4);

        RollingPivotsResult r2 = results[20];
        Assert.AreEqual(213.6367m, NullMath.Round(r2.PP, 4));
        Assert.AreEqual(212.1033m, NullMath.Round(r2.S1, 4));
        Assert.AreEqual(209.9867m, NullMath.Round(r2.S2, 4));
        Assert.AreEqual(208.4533m, NullMath.Round(r2.S3, 4));
        Assert.AreEqual(null, r2.S4);
        Assert.AreEqual(215.7533m, NullMath.Round(r2.R1, 4));
        Assert.AreEqual(217.2867m, NullMath.Round(r2.R2, 4));
        Assert.AreEqual(219.4033m, NullMath.Round(r2.R3, 4));
        Assert.AreEqual(null, r2.R4);

        RollingPivotsResult r3 = results[149];
        Assert.AreEqual(233.6333m, NullMath.Round(r3.PP, 4));
        Assert.AreEqual(231.3567m, NullMath.Round(r3.S1, 4));
        Assert.AreEqual(227.3733m, NullMath.Round(r3.S2, 4));
        Assert.AreEqual(225.0967m, NullMath.Round(r3.S3, 4));
        Assert.AreEqual(null, r3.S4);
        Assert.AreEqual(237.6167m, NullMath.Round(r3.R1, 4));
        Assert.AreEqual(239.8933m, NullMath.Round(r3.R2, 4));
        Assert.AreEqual(243.8767m, NullMath.Round(r3.R3, 4));
        Assert.AreEqual(null, r3.R4);

        RollingPivotsResult r4 = results[249];
        Assert.AreEqual(253.9533m, NullMath.Round(r4.PP, 4));
        Assert.AreEqual(251.5267m, NullMath.Round(r4.S1, 4));
        Assert.AreEqual(247.4433m, NullMath.Round(r4.S2, 4));
        Assert.AreEqual(245.0167m, NullMath.Round(r4.S3, 4));
        Assert.AreEqual(null, r4.S4);
        Assert.AreEqual(258.0367m, NullMath.Round(r4.R1, 4));
        Assert.AreEqual(260.4633m, NullMath.Round(r4.R2, 4));
        Assert.AreEqual(264.5467m, NullMath.Round(r4.R3, 4));
        Assert.AreEqual(null, r4.R4);

        RollingPivotsResult r5 = results[501];
        Assert.AreEqual(260.0267m, NullMath.Round(r5.PP, 4));
        Assert.AreEqual(246.4633m, NullMath.Round(r5.S1, 4));
        Assert.AreEqual(238.7767m, NullMath.Round(r5.S2, 4));
        Assert.AreEqual(225.2133m, NullMath.Round(r5.S3, 4));
        Assert.AreEqual(null, r5.S4);
        Assert.AreEqual(267.7133m, NullMath.Round(r5.R1, 4));
        Assert.AreEqual(281.2767m, NullMath.Round(r5.R2, 4));
        Assert.AreEqual(288.9633m, NullMath.Round(r5.R3, 4));
        Assert.AreEqual(null, r5.R4);
    }

    [TestMethod]
    public void Camarilla()
    {
        int windowPeriods = 10;
        int offsetPeriods = 0;
        PivotPointType pointType = PivotPointType.Camarilla;

        IEnumerable<Quote> h = TestData.GetDefault(38);
        List<RollingPivotsResult> results =
            Indicator.GetRollingPivots(h, windowPeriods, offsetPeriods, pointType)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(38, results.Count);
        Assert.AreEqual(28, results.Count(x => x.PP != null));

        // sample values
        RollingPivotsResult r1 = results[9];
        Assert.AreEqual(null, r1.R4);
        Assert.AreEqual(null, r1.R3);
        Assert.AreEqual(null, r1.PP);
        Assert.AreEqual(null, r1.S1);
        Assert.AreEqual(null, r1.S2);
        Assert.AreEqual(null, r1.R1);
        Assert.AreEqual(null, r1.R2);
        Assert.AreEqual(null, r1.S3);
        Assert.AreEqual(null, r1.S4);

        RollingPivotsResult r2 = results[10];
        Assert.AreEqual(267.0800m, NullMath.Round(r2.PP, 4));
        Assert.AreEqual(265.8095m, NullMath.Round(r2.S1, 4));
        Assert.AreEqual(264.5390m, NullMath.Round(r2.S2, 4));
        Assert.AreEqual(263.2685m, NullMath.Round(r2.S3, 4));
        Assert.AreEqual(259.4570m, NullMath.Round(r2.S4, 4));
        Assert.AreEqual(268.3505m, NullMath.Round(r2.R1, 4));
        Assert.AreEqual(269.6210m, NullMath.Round(r2.R2, 4));
        Assert.AreEqual(270.8915m, NullMath.Round(r2.R3, 4));
        Assert.AreEqual(274.7030m, NullMath.Round(r2.R4, 4));

        RollingPivotsResult r3 = results[22];
        Assert.AreEqual(263.2900m, NullMath.Round(r3.PP, 4));
        Assert.AreEqual(261.6840m, NullMath.Round(r3.S1, 4));
        Assert.AreEqual(260.0780m, NullMath.Round(r3.S2, 4));
        Assert.AreEqual(258.4720m, NullMath.Round(r3.S3, 4));
        Assert.AreEqual(253.6540m, NullMath.Round(r3.S4, 4));
        Assert.AreEqual(264.8960m, NullMath.Round(r3.R1, 4));
        Assert.AreEqual(266.5020m, NullMath.Round(r3.R2, 4));
        Assert.AreEqual(268.1080m, NullMath.Round(r3.R3, 4));
        Assert.AreEqual(272.9260m, NullMath.Round(r3.R4, 4));

        RollingPivotsResult r4 = results[23];
        Assert.AreEqual(257.1700m, NullMath.Round(r4.PP, 4));
        Assert.AreEqual(255.5640m, NullMath.Round(r4.S1, 4));
        Assert.AreEqual(253.9580m, NullMath.Round(r4.S2, 4));
        Assert.AreEqual(252.3520m, NullMath.Round(r4.S3, 4));
        Assert.AreEqual(247.5340m, NullMath.Round(r4.S4, 4));
        Assert.AreEqual(258.7760m, NullMath.Round(r4.R1, 4));
        Assert.AreEqual(260.3820m, NullMath.Round(r4.R2, 4));
        Assert.AreEqual(261.9880m, NullMath.Round(r4.R3, 4));
        Assert.AreEqual(266.8060m, NullMath.Round(r4.R4, 4));

        RollingPivotsResult r5 = results[37];
        Assert.AreEqual(243.1500m, NullMath.Round(r5.PP, 4));
        Assert.AreEqual(240.5650m, NullMath.Round(r5.S1, 4));
        Assert.AreEqual(237.9800m, NullMath.Round(r5.S2, 4));
        Assert.AreEqual(235.3950m, NullMath.Round(r5.S3, 4));
        Assert.AreEqual(227.6400m, NullMath.Round(r5.S4, 4));
        Assert.AreEqual(245.7350m, NullMath.Round(r5.R1, 4));
        Assert.AreEqual(248.3200m, NullMath.Round(r5.R2, 4));
        Assert.AreEqual(250.9050m, NullMath.Round(r5.R3, 4));
        Assert.AreEqual(258.6600m, NullMath.Round(r5.R4, 4));
    }

    [TestMethod]
    public void Demark()
    {
        int windowPeriods = 10;
        int offsetPeriods = 10;
        PivotPointType pointType = PivotPointType.Demark;

        List<RollingPivotsResult> results =
            Indicator.GetRollingPivots(quotes, windowPeriods, offsetPeriods, pointType)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.PP != null));

        // sample values
        RollingPivotsResult r1 = results[19];
        Assert.AreEqual(null, r1.PP);
        Assert.AreEqual(null, r1.S1);
        Assert.AreEqual(null, r1.S2);
        Assert.AreEqual(null, r1.S3);
        Assert.AreEqual(null, r1.S4);
        Assert.AreEqual(null, r1.R1);
        Assert.AreEqual(null, r1.R2);
        Assert.AreEqual(null, r1.R3);
        Assert.AreEqual(null, r1.R4);

        RollingPivotsResult r2 = results[20];
        Assert.AreEqual(212.9900m, NullMath.Round(r2.PP, 4));
        Assert.AreEqual(210.8100m, NullMath.Round(r2.S1, 4));
        Assert.AreEqual(214.4600m, NullMath.Round(r2.R1, 4));
        Assert.AreEqual(null, r2.R2);
        Assert.AreEqual(null, r2.R3);
        Assert.AreEqual(null, r2.R4);
        Assert.AreEqual(null, r2.S2);
        Assert.AreEqual(null, r2.S3);
        Assert.AreEqual(null, r2.S4);

        RollingPivotsResult r3 = results[149];
        Assert.AreEqual(232.6525m, NullMath.Round(r3.PP, 4));
        Assert.AreEqual(229.3950m, NullMath.Round(r3.S1, 4));
        Assert.AreEqual(235.6550m, NullMath.Round(r3.R1, 4));
        Assert.AreEqual(null, r3.R2);
        Assert.AreEqual(null, r3.R3);
        Assert.AreEqual(null, r3.R4);
        Assert.AreEqual(null, r3.S2);
        Assert.AreEqual(null, r3.S3);
        Assert.AreEqual(null, r3.S4);

        RollingPivotsResult r4 = results[250];
        Assert.AreEqual(252.9325m, NullMath.Round(r4.PP, 4));
        Assert.AreEqual(249.4850m, NullMath.Round(r4.S1, 4));
        Assert.AreEqual(255.9950m, NullMath.Round(r4.R1, 4));
        Assert.AreEqual(null, r4.R2);
        Assert.AreEqual(null, r4.R3);
        Assert.AreEqual(null, r4.R4);
        Assert.AreEqual(null, r4.S2);
        Assert.AreEqual(null, r4.S3);
        Assert.AreEqual(null, r4.S4);

        RollingPivotsResult r5 = results[251];
        Assert.AreEqual(252.6700m, NullMath.Round(r5.PP, 4));
        Assert.AreEqual(248.9600m, NullMath.Round(r5.S1, 4));
        Assert.AreEqual(255.4700m, NullMath.Round(r5.R1, 4));
        Assert.AreEqual(null, r5.R2);
        Assert.AreEqual(null, r5.R3);
        Assert.AreEqual(null, r5.R4);
        Assert.AreEqual(null, r5.S2);
        Assert.AreEqual(null, r5.S3);
        Assert.AreEqual(null, r5.S4);

        RollingPivotsResult r6 = results[501];
        Assert.AreEqual(264.6125m, NullMath.Round(r6.PP, 4));
        Assert.AreEqual(255.6350m, NullMath.Round(r6.S1, 4));
        Assert.AreEqual(276.8850m, NullMath.Round(r6.R1, 4));
        Assert.AreEqual(null, r6.R2);
        Assert.AreEqual(null, r6.R3);
        Assert.AreEqual(null, r6.R4);
        Assert.AreEqual(null, r6.S2);
        Assert.AreEqual(null, r6.S3);
        Assert.AreEqual(null, r6.S4);
    }

    [TestMethod]
    public void Fibonacci()
    {
        int windowPeriods = 44;
        int offsetPeriods = 15;
        PivotPointType pointType = PivotPointType.Fibonacci;

        IEnumerable<Quote> h = TestData.GetIntraday(300);
        List<RollingPivotsResult> results =
            Indicator.GetRollingPivots(h, windowPeriods, offsetPeriods, pointType)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(300, results.Count);
        Assert.AreEqual(241, results.Count(x => x.PP != null));

        // sample values
        RollingPivotsResult r1 = results[58];
        Assert.AreEqual(null, r1.R4);
        Assert.AreEqual(null, r1.R3);
        Assert.AreEqual(null, r1.PP);
        Assert.AreEqual(null, r1.S1);
        Assert.AreEqual(null, r1.S2);
        Assert.AreEqual(null, r1.R1);
        Assert.AreEqual(null, r1.R2);
        Assert.AreEqual(null, r1.S3);
        Assert.AreEqual(null, r1.S4);

        RollingPivotsResult r2 = results[59];
        Assert.AreEqual(368.4283m, NullMath.Round(r2.PP, 4));
        Assert.AreEqual(367.8553m, NullMath.Round(r2.S1, 4));
        Assert.AreEqual(367.5013m, NullMath.Round(r2.S2, 4));
        Assert.AreEqual(366.9283m, NullMath.Round(r2.S3, 4));
        Assert.AreEqual(369.0013m, NullMath.Round(r2.R1, 4));
        Assert.AreEqual(369.3553m, NullMath.Round(r2.R2, 4));
        Assert.AreEqual(369.9283m, NullMath.Round(r2.R3, 4));

        RollingPivotsResult r3 = results[118];
        Assert.AreEqual(369.1573m, NullMath.Round(r3.PP, 4));
        Assert.AreEqual(368.7333m, NullMath.Round(r3.S1, 4));
        Assert.AreEqual(368.4713m, NullMath.Round(r3.S2, 4));
        Assert.AreEqual(368.0473m, NullMath.Round(r3.S3, 4));
        Assert.AreEqual(369.5813m, NullMath.Round(r3.R1, 4));
        Assert.AreEqual(369.8433m, NullMath.Round(r3.R2, 4));
        Assert.AreEqual(370.2673m, NullMath.Round(r3.R3, 4));

        RollingPivotsResult r4 = results[119];
        Assert.AreEqual(369.1533m, NullMath.Round(r4.PP, 4));
        Assert.AreEqual(368.7293m, NullMath.Round(r4.S1, 4));
        Assert.AreEqual(368.4674m, NullMath.Round(r4.S2, 4));
        Assert.AreEqual(368.0433m, NullMath.Round(r4.S3, 4));
        Assert.AreEqual(369.5774m, NullMath.Round(r4.R1, 4));
        Assert.AreEqual(369.8393m, NullMath.Round(r4.R2, 4));
        Assert.AreEqual(370.2633m, NullMath.Round(r4.R3, 4));

        RollingPivotsResult r5 = results[149];
        Assert.AreEqual(369.0183m, NullMath.Round(r5.PP, 4));
        Assert.AreEqual(368.6593m, NullMath.Round(r5.S1, 4));
        Assert.AreEqual(368.4374m, NullMath.Round(r5.S2, 4));
        Assert.AreEqual(368.0783m, NullMath.Round(r5.S3, 4));
        Assert.AreEqual(369.3774m, NullMath.Round(r5.R1, 4));
        Assert.AreEqual(369.5993m, NullMath.Round(r5.R2, 4));
        Assert.AreEqual(369.9583m, NullMath.Round(r5.R3, 4));

        RollingPivotsResult r6 = results[299];
        Assert.AreEqual(367.7567m, NullMath.Round(r6.PP, 4));
        Assert.AreEqual(367.3174m, NullMath.Round(r6.S1, 4));
        Assert.AreEqual(367.0460m, NullMath.Round(r6.S2, 4));
        Assert.AreEqual(366.6067m, NullMath.Round(r6.S3, 4));
        Assert.AreEqual(368.1960m, NullMath.Round(r6.R1, 4));
        Assert.AreEqual(368.4674m, NullMath.Round(r6.R2, 4));
        Assert.AreEqual(368.9067m, NullMath.Round(r6.R3, 4));
    }

    [TestMethod]
    public void Woodie()
    {
        int windowPeriods = 375;
        int offsetPeriods = 16;
        PivotPointType pointType = PivotPointType.Woodie;

        IEnumerable<Quote> h = TestData.GetIntraday(1564);
        List<RollingPivotsResult> results =
            Indicator.GetRollingPivots(h, windowPeriods, offsetPeriods, pointType)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(1564, results.Count);
        Assert.AreEqual(1173, results.Count(x => x.PP != null));

        // sample values
        RollingPivotsResult r2 = results[390];
        Assert.AreEqual(null, r2.R4);
        Assert.AreEqual(null, r2.R3);
        Assert.AreEqual(null, r2.PP);
        Assert.AreEqual(null, r2.S1);
        Assert.AreEqual(null, r2.S2);
        Assert.AreEqual(null, r2.R1);
        Assert.AreEqual(null, r2.R2);
        Assert.AreEqual(null, r2.S3);
        Assert.AreEqual(null, r2.S4);

        RollingPivotsResult r3 = results[391];
        Assert.AreEqual(368.7850m, NullMath.Round(r3.PP, 4));
        Assert.AreEqual(367.9901m, NullMath.Round(r3.S1, 4));
        Assert.AreEqual(365.1252m, NullMath.Round(r3.S2, 4));
        Assert.AreEqual(364.3303m, NullMath.Round(r3.S3, 4));
        Assert.AreEqual(371.6499m, NullMath.Round(r3.R1, 4));
        Assert.AreEqual(372.4448m, NullMath.Round(r3.R2, 4));
        Assert.AreEqual(375.3097m, NullMath.Round(r3.R3, 4));

        RollingPivotsResult r4 = results[1172];
        Assert.AreEqual(371.75m, NullMath.Round(r4.PP, 4));
        Assert.AreEqual(371.04m, NullMath.Round(r4.S1, 4));
        Assert.AreEqual(369.35m, NullMath.Round(r4.S2, 4));
        Assert.AreEqual(368.64m, NullMath.Round(r4.S3, 4));
        Assert.AreEqual(373.44m, NullMath.Round(r4.R1, 4));
        Assert.AreEqual(374.15m, NullMath.Round(r4.R2, 4));
        Assert.AreEqual(375.84m, NullMath.Round(r4.R3, 4));

        RollingPivotsResult r5 = results[1173];
        Assert.AreEqual(371.3625m, NullMath.Round(r5.PP, 4));
        Assert.AreEqual(370.2650m, NullMath.Round(r5.S1, 4));
        Assert.AreEqual(369.9525m, NullMath.Round(r5.S2, 4));
        Assert.AreEqual(368.8550m, NullMath.Round(r5.S3, 4));
        Assert.AreEqual(371.6750m, NullMath.Round(r5.R1, 4));
        Assert.AreEqual(372.7725m, NullMath.Round(r5.R2, 4));
        Assert.AreEqual(373.0850m, NullMath.Round(r5.R3, 4));

        RollingPivotsResult r6 = results[1563];
        Assert.AreEqual(369.38m, NullMath.Round(r6.PP, 4));
        Assert.AreEqual(366.52m, NullMath.Round(r6.S1, 4));
        Assert.AreEqual(364.16m, NullMath.Round(r6.S2, 4));
        Assert.AreEqual(361.30m, NullMath.Round(r6.S3, 4));
        Assert.AreEqual(371.74m, NullMath.Round(r6.R1, 4));
        Assert.AreEqual(374.60m, NullMath.Round(r6.R2, 4));
        Assert.AreEqual(376.96m, NullMath.Round(r6.R3, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<RollingPivotsResult> r = Indicator.GetRollingPivots(badQuotes, 5, 5);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<RollingPivotsResult> r0 = noquotes.GetRollingPivots(5, 2);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<RollingPivotsResult> r1 = onequote.GetRollingPivots(5, 2);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        int windowPeriods = 11;
        int offsetPeriods = 9;
        PivotPointType pointType = PivotPointType.Standard;

        List<RollingPivotsResult> results =
            quotes.GetRollingPivots(windowPeriods, offsetPeriods, pointType)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(502 - (windowPeriods + offsetPeriods), results.Count);

        RollingPivotsResult last = results.LastOrDefault();
        Assert.AreEqual(260.0267m, NullMath.Round(last.PP, 4));
        Assert.AreEqual(246.4633m, NullMath.Round(last.S1, 4));
        Assert.AreEqual(238.7767m, NullMath.Round(last.S2, 4));
        Assert.AreEqual(225.2133m, NullMath.Round(last.S3, 4));
        Assert.AreEqual(null, last.S4);
        Assert.AreEqual(267.7133m, NullMath.Round(last.R1, 4));
        Assert.AreEqual(281.2767m, NullMath.Round(last.R2, 4));
        Assert.AreEqual(288.9633m, NullMath.Round(last.R3, 4));
        Assert.AreEqual(null, last.R4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad window period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetRollingPivots(quotes, 0, 10));

        // bad offset period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetRollingPivots(quotes, 10, -1));
    }
}
