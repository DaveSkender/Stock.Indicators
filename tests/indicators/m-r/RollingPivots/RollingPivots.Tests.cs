using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class RollingPivotsTests : TestBase
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

        // proper quantities
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
        Assert.AreEqual(213.6367m, r2.PP.Round(4));
        Assert.AreEqual(212.1033m, r2.S1.Round(4));
        Assert.AreEqual(209.9867m, r2.S2.Round(4));
        Assert.AreEqual(208.4533m, r2.S3.Round(4));
        Assert.AreEqual(null, r2.S4);
        Assert.AreEqual(215.7533m, r2.R1.Round(4));
        Assert.AreEqual(217.2867m, r2.R2.Round(4));
        Assert.AreEqual(219.4033m, r2.R3.Round(4));
        Assert.AreEqual(null, r2.R4);

        RollingPivotsResult r3 = results[149];
        Assert.AreEqual(233.6333m, r3.PP.Round(4));
        Assert.AreEqual(231.3567m, r3.S1.Round(4));
        Assert.AreEqual(227.3733m, r3.S2.Round(4));
        Assert.AreEqual(225.0967m, r3.S3.Round(4));
        Assert.AreEqual(null, r3.S4);
        Assert.AreEqual(237.6167m, r3.R1.Round(4));
        Assert.AreEqual(239.8933m, r3.R2.Round(4));
        Assert.AreEqual(243.8767m, r3.R3.Round(4));
        Assert.AreEqual(null, r3.R4);

        RollingPivotsResult r4 = results[249];
        Assert.AreEqual(253.9533m, r4.PP.Round(4));
        Assert.AreEqual(251.5267m, r4.S1.Round(4));
        Assert.AreEqual(247.4433m, r4.S2.Round(4));
        Assert.AreEqual(245.0167m, r4.S3.Round(4));
        Assert.AreEqual(null, r4.S4);
        Assert.AreEqual(258.0367m, r4.R1.Round(4));
        Assert.AreEqual(260.4633m, r4.R2.Round(4));
        Assert.AreEqual(264.5467m, r4.R3.Round(4));
        Assert.AreEqual(null, r4.R4);

        RollingPivotsResult r5 = results[501];
        Assert.AreEqual(260.0267m, r5.PP.Round(4));
        Assert.AreEqual(246.4633m, r5.S1.Round(4));
        Assert.AreEqual(238.7767m, r5.S2.Round(4));
        Assert.AreEqual(225.2133m, r5.S3.Round(4));
        Assert.AreEqual(null, r5.S4);
        Assert.AreEqual(267.7133m, r5.R1.Round(4));
        Assert.AreEqual(281.2767m, r5.R2.Round(4));
        Assert.AreEqual(288.9633m, r5.R3.Round(4));
        Assert.AreEqual(null, r5.R4);
    }

    [TestMethod]
    public void Camarilla()
    {
        int windowPeriods = 10;
        int offsetPeriods = 0;
        PivotPointType pointType = PivotPointType.Camarilla;

        IEnumerable<Quote> h = TestData.GetDefault(38);

        List<RollingPivotsResult> results = h
            .GetRollingPivots(windowPeriods, offsetPeriods, pointType)
            .ToList();

        // proper quantities
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
        Assert.AreEqual(267.0800m, r2.PP.Round(4));
        Assert.AreEqual(265.8095m, r2.S1.Round(4));
        Assert.AreEqual(264.5390m, r2.S2.Round(4));
        Assert.AreEqual(263.2685m, r2.S3.Round(4));
        Assert.AreEqual(259.4570m, r2.S4.Round(4));
        Assert.AreEqual(268.3505m, r2.R1.Round(4));
        Assert.AreEqual(269.6210m, r2.R2.Round(4));
        Assert.AreEqual(270.8915m, r2.R3.Round(4));
        Assert.AreEqual(274.7030m, r2.R4.Round(4));

        RollingPivotsResult r3 = results[22];
        Assert.AreEqual(263.2900m, r3.PP.Round(4));
        Assert.AreEqual(261.6840m, r3.S1.Round(4));
        Assert.AreEqual(260.0780m, r3.S2.Round(4));
        Assert.AreEqual(258.4720m, r3.S3.Round(4));
        Assert.AreEqual(253.6540m, r3.S4.Round(4));
        Assert.AreEqual(264.8960m, r3.R1.Round(4));
        Assert.AreEqual(266.5020m, r3.R2.Round(4));
        Assert.AreEqual(268.1080m, r3.R3.Round(4));
        Assert.AreEqual(272.9260m, r3.R4.Round(4));

        RollingPivotsResult r4 = results[23];
        Assert.AreEqual(257.1700m, r4.PP.Round(4));
        Assert.AreEqual(255.5640m, r4.S1.Round(4));
        Assert.AreEqual(253.9580m, r4.S2.Round(4));
        Assert.AreEqual(252.3520m, r4.S3.Round(4));
        Assert.AreEqual(247.5340m, r4.S4.Round(4));
        Assert.AreEqual(258.7760m, r4.R1.Round(4));
        Assert.AreEqual(260.3820m, r4.R2.Round(4));
        Assert.AreEqual(261.9880m, r4.R3.Round(4));
        Assert.AreEqual(266.8060m, r4.R4.Round(4));

        RollingPivotsResult r5 = results[37];
        Assert.AreEqual(243.1500m, r5.PP.Round(4));
        Assert.AreEqual(240.5650m, r5.S1.Round(4));
        Assert.AreEqual(237.9800m, r5.S2.Round(4));
        Assert.AreEqual(235.3950m, r5.S3.Round(4));
        Assert.AreEqual(227.6400m, r5.S4.Round(4));
        Assert.AreEqual(245.7350m, r5.R1.Round(4));
        Assert.AreEqual(248.3200m, r5.R2.Round(4));
        Assert.AreEqual(250.9050m, r5.R3.Round(4));
        Assert.AreEqual(258.6600m, r5.R4.Round(4));
    }

    [TestMethod]
    public void Demark()
    {
        int windowPeriods = 10;
        int offsetPeriods = 10;
        PivotPointType pointType = PivotPointType.Demark;

        List<RollingPivotsResult> results = quotes
            .GetRollingPivots(windowPeriods, offsetPeriods, pointType)
            .ToList();

        // proper quantities
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
        Assert.AreEqual(212.9900m, r2.PP.Round(4));
        Assert.AreEqual(210.8100m, r2.S1.Round(4));
        Assert.AreEqual(214.4600m, r2.R1.Round(4));
        Assert.AreEqual(null, r2.R2);
        Assert.AreEqual(null, r2.R3);
        Assert.AreEqual(null, r2.R4);
        Assert.AreEqual(null, r2.S2);
        Assert.AreEqual(null, r2.S3);
        Assert.AreEqual(null, r2.S4);

        RollingPivotsResult r3 = results[149];
        Assert.AreEqual(232.6525m, r3.PP.Round(4));
        Assert.AreEqual(229.3950m, r3.S1.Round(4));
        Assert.AreEqual(235.6550m, r3.R1.Round(4));
        Assert.AreEqual(null, r3.R2);
        Assert.AreEqual(null, r3.R3);
        Assert.AreEqual(null, r3.R4);
        Assert.AreEqual(null, r3.S2);
        Assert.AreEqual(null, r3.S3);
        Assert.AreEqual(null, r3.S4);

        RollingPivotsResult r4 = results[250];
        Assert.AreEqual(252.9325m, r4.PP.Round(4));
        Assert.AreEqual(249.4850m, r4.S1.Round(4));
        Assert.AreEqual(255.9950m, r4.R1.Round(4));
        Assert.AreEqual(null, r4.R2);
        Assert.AreEqual(null, r4.R3);
        Assert.AreEqual(null, r4.R4);
        Assert.AreEqual(null, r4.S2);
        Assert.AreEqual(null, r4.S3);
        Assert.AreEqual(null, r4.S4);

        RollingPivotsResult r5 = results[251];
        Assert.AreEqual(252.6700m, r5.PP.Round(4));
        Assert.AreEqual(248.9600m, r5.S1.Round(4));
        Assert.AreEqual(255.4700m, r5.R1.Round(4));
        Assert.AreEqual(null, r5.R2);
        Assert.AreEqual(null, r5.R3);
        Assert.AreEqual(null, r5.R4);
        Assert.AreEqual(null, r5.S2);
        Assert.AreEqual(null, r5.S3);
        Assert.AreEqual(null, r5.S4);

        RollingPivotsResult r6 = results[501];
        Assert.AreEqual(264.6125m, r6.PP.Round(4));
        Assert.AreEqual(255.6350m, r6.S1.Round(4));
        Assert.AreEqual(276.8850m, r6.R1.Round(4));
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
            h.GetRollingPivots(windowPeriods, offsetPeriods, pointType)
            .ToList();

        // proper quantities
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
        Assert.AreEqual(368.4283m, r2.PP.Round(4));
        Assert.AreEqual(367.8553m, r2.S1.Round(4));
        Assert.AreEqual(367.5013m, r2.S2.Round(4));
        Assert.AreEqual(366.9283m, r2.S3.Round(4));
        Assert.AreEqual(369.0013m, r2.R1.Round(4));
        Assert.AreEqual(369.3553m, r2.R2.Round(4));
        Assert.AreEqual(369.9283m, r2.R3.Round(4));

        RollingPivotsResult r3 = results[118];
        Assert.AreEqual(369.1573m, r3.PP.Round(4));
        Assert.AreEqual(368.7333m, r3.S1.Round(4));
        Assert.AreEqual(368.4713m, r3.S2.Round(4));
        Assert.AreEqual(368.0473m, r3.S3.Round(4));
        Assert.AreEqual(369.5813m, r3.R1.Round(4));
        Assert.AreEqual(369.8433m, r3.R2.Round(4));
        Assert.AreEqual(370.2673m, r3.R3.Round(4));

        RollingPivotsResult r4 = results[119];
        Assert.AreEqual(369.1533m, r4.PP.Round(4));
        Assert.AreEqual(368.7293m, r4.S1.Round(4));
        Assert.AreEqual(368.4674m, r4.S2.Round(4));
        Assert.AreEqual(368.0433m, r4.S3.Round(4));
        Assert.AreEqual(369.5774m, r4.R1.Round(4));
        Assert.AreEqual(369.8393m, r4.R2.Round(4));
        Assert.AreEqual(370.2633m, r4.R3.Round(4));

        RollingPivotsResult r5 = results[149];
        Assert.AreEqual(369.0183m, r5.PP.Round(4));
        Assert.AreEqual(368.6593m, r5.S1.Round(4));
        Assert.AreEqual(368.4374m, r5.S2.Round(4));
        Assert.AreEqual(368.0783m, r5.S3.Round(4));
        Assert.AreEqual(369.3774m, r5.R1.Round(4));
        Assert.AreEqual(369.5993m, r5.R2.Round(4));
        Assert.AreEqual(369.9583m, r5.R3.Round(4));

        RollingPivotsResult r6 = results[299];
        Assert.AreEqual(367.7567m, r6.PP.Round(4));
        Assert.AreEqual(367.3174m, r6.S1.Round(4));
        Assert.AreEqual(367.0460m, r6.S2.Round(4));
        Assert.AreEqual(366.6067m, r6.S3.Round(4));
        Assert.AreEqual(368.1960m, r6.R1.Round(4));
        Assert.AreEqual(368.4674m, r6.R2.Round(4));
        Assert.AreEqual(368.9067m, r6.R3.Round(4));
    }

    [TestMethod]
    public void Woodie()
    {
        int windowPeriods = 375;
        int offsetPeriods = 16;
        PivotPointType pointType = PivotPointType.Woodie;

        IEnumerable<Quote> h = TestData.GetIntraday(1564);

        List<RollingPivotsResult> results = h
            .GetRollingPivots(windowPeriods, offsetPeriods, pointType)
            .ToList();

        // proper quantities
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
        Assert.AreEqual(368.7850m, r3.PP.Round(4));
        Assert.AreEqual(367.9901m, r3.S1.Round(4));
        Assert.AreEqual(365.1252m, r3.S2.Round(4));
        Assert.AreEqual(364.3303m, r3.S3.Round(4));
        Assert.AreEqual(371.6499m, r3.R1.Round(4));
        Assert.AreEqual(372.4448m, r3.R2.Round(4));
        Assert.AreEqual(375.3097m, r3.R3.Round(4));

        RollingPivotsResult r4 = results[1172];
        Assert.AreEqual(371.75m, r4.PP.Round(4));
        Assert.AreEqual(371.04m, r4.S1.Round(4));
        Assert.AreEqual(369.35m, r4.S2.Round(4));
        Assert.AreEqual(368.64m, r4.S3.Round(4));
        Assert.AreEqual(373.44m, r4.R1.Round(4));
        Assert.AreEqual(374.15m, r4.R2.Round(4));
        Assert.AreEqual(375.84m, r4.R3.Round(4));

        RollingPivotsResult r5 = results[1173];
        Assert.AreEqual(371.3625m, r5.PP.Round(4));
        Assert.AreEqual(370.2650m, r5.S1.Round(4));
        Assert.AreEqual(369.9525m, r5.S2.Round(4));
        Assert.AreEqual(368.8550m, r5.S3.Round(4));
        Assert.AreEqual(371.6750m, r5.R1.Round(4));
        Assert.AreEqual(372.7725m, r5.R2.Round(4));
        Assert.AreEqual(373.0850m, r5.R3.Round(4));

        RollingPivotsResult r6 = results[1563];
        Assert.AreEqual(369.38m, r6.PP.Round(4));
        Assert.AreEqual(366.52m, r6.S1.Round(4));
        Assert.AreEqual(364.16m, r6.S2.Round(4));
        Assert.AreEqual(361.30m, r6.S3.Round(4));
        Assert.AreEqual(371.74m, r6.R1.Round(4));
        Assert.AreEqual(374.60m, r6.R2.Round(4));
        Assert.AreEqual(376.96m, r6.R3.Round(4));
    }

    [TestMethod]
    public void BadData()
    {
        List<RollingPivotsResult> r = badQuotes
            .GetRollingPivots(5, 5)
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<RollingPivotsResult> r0 = noquotes
            .GetRollingPivots(5, 2)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<RollingPivotsResult> r1 = onequote
            .GetRollingPivots(5, 2)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int windowPeriods = 11;
        int offsetPeriods = 9;
        PivotPointType pointType = PivotPointType.Standard;

        List<RollingPivotsResult> results = quotes
            .GetRollingPivots(windowPeriods, offsetPeriods, pointType)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (windowPeriods + offsetPeriods), results.Count);

        RollingPivotsResult last = results.LastOrDefault();
        Assert.AreEqual(260.0267m, last.PP.Round(4));
        Assert.AreEqual(246.4633m, last.S1.Round(4));
        Assert.AreEqual(238.7767m, last.S2.Round(4));
        Assert.AreEqual(225.2133m, last.S3.Round(4));
        Assert.AreEqual(null, last.S4);
        Assert.AreEqual(267.7133m, last.R1.Round(4));
        Assert.AreEqual(281.2767m, last.R2.Round(4));
        Assert.AreEqual(288.9633m, last.R3.Round(4));
        Assert.AreEqual(null, last.R4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad window period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetRollingPivots(0, 10));

        // bad offset period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetRollingPivots(10, -1));
    }
}
