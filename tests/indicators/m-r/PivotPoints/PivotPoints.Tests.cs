using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class PivotPointsTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        PeriodSize periodSize = PeriodSize.Month;
        PivotPointType pointType = PivotPointType.Standard;

        List<PivotPointsResult> results = quotes
            .GetPivotPoints(periodSize, pointType)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.PP != null));

        // sample values
        PivotPointsResult r1 = results[19];
        Assert.AreEqual(null, r1.PP);
        Assert.AreEqual(null, r1.S1);
        Assert.AreEqual(null, r1.S2);
        Assert.AreEqual(null, r1.S3);
        Assert.AreEqual(null, r1.S4);
        Assert.AreEqual(null, r1.R1);
        Assert.AreEqual(null, r1.R2);
        Assert.AreEqual(null, r1.R3);
        Assert.AreEqual(null, r1.R4);

        PivotPointsResult r2 = results[20];
        Assert.AreEqual(214.5000m, r2.PP.Round(4));
        Assert.AreEqual(211.9800m, r2.S1.Round(4));
        Assert.AreEqual(209.0000m, r2.S2.Round(4));
        Assert.AreEqual(206.4800m, r2.S3.Round(4));
        Assert.AreEqual(null, r2.S4);
        Assert.AreEqual(217.4800m, r2.R1.Round(4));
        Assert.AreEqual(220.0000m, r2.R2.Round(4));
        Assert.AreEqual(222.9800m, r2.R3.Round(4));
        Assert.AreEqual(null, r2.R4);

        PivotPointsResult r3 = results[149];
        Assert.AreEqual(233.6400m, r3.PP.Round(4));
        Assert.AreEqual(230.8100m, r3.S1.Round(4));
        Assert.AreEqual(226.3300m, r3.S2.Round(4));
        Assert.AreEqual(223.5000m, r3.S3.Round(4));
        Assert.AreEqual(null, r3.S4);
        Assert.AreEqual(238.1200m, r3.R1.Round(4));
        Assert.AreEqual(240.9500m, r3.R2.Round(4));
        Assert.AreEqual(245.4300m, r3.R3.Round(4));
        Assert.AreEqual(null, r3.R4);

        PivotPointsResult r4 = results[250];
        Assert.AreEqual(251.2767m, r4.PP.Round(4));
        Assert.AreEqual(247.6133m, r4.S1.Round(4));
        Assert.AreEqual(241.2867m, r4.S2.Round(4));
        Assert.AreEqual(237.6233m, r4.S3.Round(4));
        Assert.AreEqual(null, r4.S4);
        Assert.AreEqual(257.6033m, r4.R1.Round(4));
        Assert.AreEqual(261.2667m, r4.R2.Round(4));
        Assert.AreEqual(267.5933m, r4.R3.Round(4));
        Assert.AreEqual(null, r4.R4);

        PivotPointsResult r5 = results[251];
        Assert.AreEqual(255.1967m, r5.PP.Round(4));
        Assert.AreEqual(251.6933m, r5.S1.Round(4));
        Assert.AreEqual(246.3667m, r5.S2.Round(4));
        Assert.AreEqual(242.8633m, r5.S3.Round(4));
        Assert.AreEqual(null, r5.S4);
        Assert.AreEqual(260.5233m, r5.R1.Round(4));
        Assert.AreEqual(264.0267m, r5.R2.Round(4));
        Assert.AreEqual(269.3533m, r5.R3.Round(4));
        Assert.AreEqual(null, r5.R4);

        PivotPointsResult r6 = results[501];
        Assert.AreEqual(266.6767m, r6.PP.Round(4));
        Assert.AreEqual(258.9633m, r6.S1.Round(4));
        Assert.AreEqual(248.9667m, r6.S2.Round(4));
        Assert.AreEqual(241.2533m, r6.S3.Round(4));
        Assert.AreEqual(null, r6.S4);
        Assert.AreEqual(276.6733m, r6.R1.Round(4));
        Assert.AreEqual(284.3867m, r6.R2.Round(4));
        Assert.AreEqual(294.3833m, r6.R3.Round(4));
        Assert.AreEqual(null, r6.R4);
    }

    [TestMethod]
    public void Camarilla()
    {
        PeriodSize periodSize = PeriodSize.Week;
        PivotPointType pointType = PivotPointType.Camarilla;

        IEnumerable<Quote> h = TestData.GetDefault(38);
        List<PivotPointsResult> results = h.GetPivotPoints(periodSize, pointType)
            .ToList();

        // proper quantities
        Assert.AreEqual(38, results.Count);
        Assert.AreEqual(33, results.Count(x => x.PP != null));

        // sample values
        PivotPointsResult r2 = results[4];
        Assert.AreEqual(null, r2.R4);
        Assert.AreEqual(null, r2.R3);
        Assert.AreEqual(null, r2.PP);
        Assert.AreEqual(null, r2.S1);
        Assert.AreEqual(null, r2.S2);
        Assert.AreEqual(null, r2.R1);
        Assert.AreEqual(null, r2.R2);
        Assert.AreEqual(null, r2.S3);
        Assert.AreEqual(null, r2.S4);

        PivotPointsResult r3 = results[5];
        Assert.AreEqual(271.0200m, r3.PP.Round(4));
        Assert.AreEqual(270.13725m, r3.S1.Round(5));
        Assert.AreEqual(269.2545m, r3.S2.Round(4));
        Assert.AreEqual(268.3718m, r3.S3.Round(4));
        Assert.AreEqual(265.7235m, r3.S4.Round(4));
        Assert.AreEqual(271.9028m, r3.R1.Round(4));
        Assert.AreEqual(272.7855m, r3.R2.Round(4));
        Assert.AreEqual(273.66825m, r3.R3.Round(5));
        Assert.AreEqual(276.3165m, r3.R4.Round(4));

        PivotPointsResult r4 = results[22];
        Assert.AreEqual(268.9600m, r4.PP.Round(4));
        Assert.AreEqual(267.9819m, r4.S1.Round(4));
        Assert.AreEqual(267.0038m, r4.S2.Round(4));
        Assert.AreEqual(266.0258m, r4.S3.Round(4));
        Assert.AreEqual(263.0915m, r4.S4.Round(4));
        Assert.AreEqual(269.9381m, r4.R1.Round(4));
        Assert.AreEqual(270.9162m, r4.R2.Round(4));
        Assert.AreEqual(271.89425m, r4.R3.Round(5));
        Assert.AreEqual(274.8285m, r4.R4.Round(4));

        PivotPointsResult r5 = results[23];
        Assert.AreEqual(257.1700m, r5.PP.Round(4));
        Assert.AreEqual(255.5640m, r5.S1.Round(4));
        Assert.AreEqual(253.9580m, r5.S2.Round(4));
        Assert.AreEqual(252.3520m, r5.S3.Round(4));
        Assert.AreEqual(247.5340m, r5.S4.Round(4));
        Assert.AreEqual(258.7760m, r5.R1.Round(4));
        Assert.AreEqual(260.3820m, r5.R2.Round(4));
        Assert.AreEqual(261.9880m, r5.R3.Round(4));
        Assert.AreEqual(266.8060m, r5.R4.Round(4));

        PivotPointsResult r6 = results[37];
        Assert.AreEqual(243.1500m, r6.PP.Round(4));
        Assert.AreEqual(241.56325m, r6.S1.Round(5));
        Assert.AreEqual(239.9765m, r6.S2.Round(4));
        Assert.AreEqual(238.3898m, r6.S3.Round(4));
        Assert.AreEqual(233.6295m, r6.S4.Round(4));
        Assert.AreEqual(244.7368m, r6.R1.Round(4));
        Assert.AreEqual(246.3235m, r6.R2.Round(4));
        Assert.AreEqual(247.91025m, r6.R3.Round(5));
        Assert.AreEqual(252.6705m, r6.R4.Round(4));
    }

    [TestMethod]
    public void Demark()
    {
        PeriodSize periodSize = PeriodSize.Month;
        PivotPointType pointType = PivotPointType.Demark;

        List<PivotPointsResult> results = quotes
            .GetPivotPoints(periodSize, pointType)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.PP != null));

        // sample values
        PivotPointsResult r1 = results[19];
        Assert.AreEqual(null, r1.R4);
        Assert.AreEqual(null, r1.R3);
        Assert.AreEqual(null, r1.PP);
        Assert.AreEqual(null, r1.S1);
        Assert.AreEqual(null, r1.S2);
        Assert.AreEqual(null, r1.R1);
        Assert.AreEqual(null, r1.R2);
        Assert.AreEqual(null, r1.S3);
        Assert.AreEqual(null, r1.S4);

        PivotPointsResult r2 = results[20];
        Assert.AreEqual(null, r2.R4);
        Assert.AreEqual(null, r2.R3);
        Assert.AreEqual(215.1300m, r2.PP.Round(4));
        Assert.AreEqual(213.2400m, r2.S1.Round(4));
        Assert.AreEqual(null, r2.S2);
        Assert.AreEqual(218.7400m, r2.R1.Round(4));
        Assert.AreEqual(null, r2.R2);
        Assert.AreEqual(null, r2.S3);
        Assert.AreEqual(null, r2.S4);

        PivotPointsResult r3 = results[149];
        Assert.AreEqual(null, r3.R4);
        Assert.AreEqual(null, r3.R3);
        Assert.AreEqual(234.3475m, r3.PP.Round(4));
        Assert.AreEqual(232.2250m, r3.S1.Round(4));
        Assert.AreEqual(null, r3.S2);
        Assert.AreEqual(239.5350m, r3.R1.Round(4));
        Assert.AreEqual(null, r3.R2);
        Assert.AreEqual(null, r3.S3);
        Assert.AreEqual(null, r3.S4);

        PivotPointsResult r4 = results[250];
        Assert.AreEqual(null, r4.R4);
        Assert.AreEqual(null, r4.R3);
        Assert.AreEqual(252.1925m, r4.PP.Round(4));
        Assert.AreEqual(249.4450m, r4.S1.Round(4));
        Assert.AreEqual(null, r4.S2);
        Assert.AreEqual(259.4350m, r4.R1.Round(4));
        Assert.AreEqual(null, r4.R2);
        Assert.AreEqual(null, r4.S3);
        Assert.AreEqual(null, r4.S4);

        PivotPointsResult r5 = results[251];
        Assert.AreEqual(null, r5.R4);
        Assert.AreEqual(null, r5.R3);
        Assert.AreEqual(256.0725m, r5.PP.Round(4));
        Assert.AreEqual(253.4450m, r5.S1.Round(4));
        Assert.AreEqual(null, r5.S2);
        Assert.AreEqual(262.2750m, r5.R1.Round(4));
        Assert.AreEqual(null, r5.R2);
        Assert.AreEqual(null, r5.S3);
        Assert.AreEqual(null, r5.S4);

        PivotPointsResult r6 = results[501];
        Assert.AreEqual(null, r6.R4);
        Assert.AreEqual(null, r6.R3);
        Assert.AreEqual(268.6050m, r6.PP.Round(4));
        Assert.AreEqual(262.8200m, r6.S1.Round(4));
        Assert.AreEqual(null, r6.S2);
        Assert.AreEqual(280.5300m, r6.R1.Round(4));
        Assert.AreEqual(null, r6.R2);
        Assert.AreEqual(null, r6.S3);
        Assert.AreEqual(null, r6.S4);

        // special Demark case: test close = open
        PivotPointsResult d1 = Indicator.GetPivotPointDemark<PivotPointsResult>(125, 200, 100, 125);
        Assert.AreEqual(550m / 4, d1.PP);
    }

    [TestMethod]
    public void Fibonacci()
    {
        PeriodSize periodSize = PeriodSize.OneHour;
        PivotPointType pointType = PivotPointType.Fibonacci;

        IEnumerable<Quote> h = TestData.GetIntraday(300);
        List<PivotPointsResult> results = h.GetPivotPoints(periodSize, pointType)
            .ToList();

        // proper quantities
        Assert.AreEqual(300, results.Count);
        Assert.AreEqual(241, results.Count(x => x.PP != null));

        // sample values
        PivotPointsResult r1 = results[58];
        Assert.AreEqual(null, r1.R4);
        Assert.AreEqual(null, r1.R3);
        Assert.AreEqual(null, r1.PP);
        Assert.AreEqual(null, r1.S1);
        Assert.AreEqual(null, r1.S2);
        Assert.AreEqual(null, r1.R1);
        Assert.AreEqual(null, r1.R2);
        Assert.AreEqual(null, r1.S3);
        Assert.AreEqual(null, r1.S4);

        PivotPointsResult r2 = results[59];
        Assert.AreEqual(368.4967m, r2.PP.Round(4));
        Assert.AreEqual(367.9237m, r2.S1.Round(4));
        Assert.AreEqual(367.5697m, r2.S2.Round(4));
        Assert.AreEqual(366.9967m, r2.S3.Round(4));
        Assert.AreEqual(369.0697m, r2.R1.Round(4));
        Assert.AreEqual(369.4237m, r2.R2.Round(4));
        Assert.AreEqual(369.9967m, r2.R3.Round(4));

        PivotPointsResult r3 = results[118];
        Assert.AreEqual(368.4967m, r3.PP.Round(4));
        Assert.AreEqual(367.9237m, r3.S1.Round(4));
        Assert.AreEqual(367.5697m, r3.S2.Round(4));
        Assert.AreEqual(366.9967m, r3.S3.Round(4));
        Assert.AreEqual(369.0697m, r3.R1.Round(4));
        Assert.AreEqual(369.4237m, r3.R2.Round(4));
        Assert.AreEqual(369.9967m, r3.R3.Round(4));

        PivotPointsResult r4 = results[119];
        Assert.AreEqual(369.0000m, r4.PP.Round(4));
        Assert.AreEqual(368.5760m, r4.S1.Round(4));
        Assert.AreEqual(368.3140m, r4.S2.Round(4));
        Assert.AreEqual(367.8900m, r4.S3.Round(4));
        Assert.AreEqual(369.4240m, r4.R1.Round(4));
        Assert.AreEqual(369.6860m, r4.R2.Round(4));
        Assert.AreEqual(370.1100m, r4.R3.Round(4));

        PivotPointsResult r5 = results[149];
        Assert.AreEqual(369.0000m, r5.PP.Round(4));
        Assert.AreEqual(368.5760m, r5.S1.Round(4));
        Assert.AreEqual(368.3140m, r5.S2.Round(4));
        Assert.AreEqual(367.8900m, r5.S3.Round(4));
        Assert.AreEqual(369.4240m, r5.R1.Round(4));
        Assert.AreEqual(369.6860m, r5.R2.Round(4));
        Assert.AreEqual(370.1100m, r5.R3.Round(4));

        PivotPointsResult r6 = results[299];
        Assert.AreEqual(368.8200m, r6.PP.Round(4));
        Assert.AreEqual(367.5632m, r6.S1.Round(4));
        Assert.AreEqual(366.7868m, r6.S2.Round(4));
        Assert.AreEqual(365.5300m, r6.S3.Round(4));
        Assert.AreEqual(370.0768m, r6.R1.Round(4));
        Assert.AreEqual(370.8532m, r6.R2.Round(4));
        Assert.AreEqual(372.1100m, r6.R3.Round(4));
    }

    [TestMethod]
    public void Woodie()
    {
        PeriodSize periodSize = PeriodSize.Day;
        PivotPointType pointType = PivotPointType.Woodie;

        IEnumerable<Quote> h = TestData.GetIntraday();
        List<PivotPointsResult> results = h.GetPivotPoints(periodSize, pointType)
            .ToList();

        // proper quantities
        Assert.AreEqual(1564, results.Count);
        Assert.AreEqual(1173, results.Count(x => x.PP != null));

        // sample values
        PivotPointsResult r2 = results[390];
        Assert.AreEqual(null, r2.R4);
        Assert.AreEqual(null, r2.R3);
        Assert.AreEqual(null, r2.PP);
        Assert.AreEqual(null, r2.S1);
        Assert.AreEqual(null, r2.S2);
        Assert.AreEqual(null, r2.R1);
        Assert.AreEqual(null, r2.R2);
        Assert.AreEqual(null, r2.S3);
        Assert.AreEqual(null, r2.S4);

        PivotPointsResult r3 = results[391];
        Assert.AreEqual(368.7875m, r3.PP.Round(4));
        Assert.AreEqual(367.9850m, r3.S1.Round(4));
        Assert.AreEqual(365.1175m, r3.S2.Round(4));
        Assert.AreEqual(364.3150m, r3.S3.Round(4));
        Assert.AreEqual(371.6550m, r3.R1.Round(4));
        Assert.AreEqual(372.4575m, r3.R2.Round(4));
        Assert.AreEqual(375.3250m, r3.R3.Round(4));

        PivotPointsResult r4 = results[1172];
        Assert.AreEqual(370.9769m, r4.PP.Round(4));
        Assert.AreEqual(370.7938m, r4.S1.Round(4));
        Assert.AreEqual(368.6845m, r4.S2.Round(4));
        Assert.AreEqual(368.5014m, r4.S3.Round(4));
        Assert.AreEqual(373.0862m, r4.R1.Round(4));
        Assert.AreEqual(373.2693m, r4.R2.Round(4));
        Assert.AreEqual(375.3786m, r4.R3.Round(4));

        PivotPointsResult r5 = results[1173];
        Assert.AreEqual(371.3625m, r5.PP.Round(4));
        Assert.AreEqual(370.2650m, r5.S1.Round(4));
        Assert.AreEqual(369.9525m, r5.S2.Round(4));
        Assert.AreEqual(368.8550m, r5.S3.Round(4));
        Assert.AreEqual(371.6750m, r5.R1.Round(4));
        Assert.AreEqual(372.7725m, r5.R2.Round(4));
        Assert.AreEqual(373.0850m, r5.R3.Round(4));

        PivotPointsResult r6 = results[1563];
        Assert.AreEqual(371.3625m, r6.PP.Round(4));
        Assert.AreEqual(370.2650m, r6.S1.Round(4));
        Assert.AreEqual(369.9525m, r6.S2.Round(4));
        Assert.AreEqual(368.8550m, r6.S3.Round(4));
        Assert.AreEqual(371.6750m, r6.R1.Round(4));
        Assert.AreEqual(372.7725m, r6.R2.Round(4));
        Assert.AreEqual(373.0850m, r6.R3.Round(4));
    }

    [TestMethod]
    public void BadData()
    {
        List<PivotPointsResult> r = badQuotes
            .GetPivotPoints(PeriodSize.Week)
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<PivotPointsResult> r0 = noquotes
            .GetPivotPoints(PeriodSize.Week)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<PivotPointsResult> r1 = onequote
            .GetPivotPoints(PeriodSize.Week)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        PeriodSize periodSize = PeriodSize.Month;
        PivotPointType pointType = PivotPointType.Standard;

        List<PivotPointsResult> results = quotes
            .GetPivotPoints(periodSize, pointType)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(482, results.Count);

        PivotPointsResult last = results.LastOrDefault();
        Assert.AreEqual(266.6767m, last.PP.Round(4));
        Assert.AreEqual(258.9633m, last.S1.Round(4));
        Assert.AreEqual(248.9667m, last.S2.Round(4));
        Assert.AreEqual(241.2533m, last.S3.Round(4));
        Assert.AreEqual(null, last.S4);
        Assert.AreEqual(276.6733m, last.R1.Round(4));
        Assert.AreEqual(284.3867m, last.R2.Round(4));
        Assert.AreEqual(294.3833m, last.R3.Round(4));
        Assert.AreEqual(null, last.R4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad pointtype size
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes
                .GetPivotPoints(PeriodSize.Week, (PivotPointType)999));

        // bad window size
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes
                .GetPivotPoints(PeriodSize.ThreeMinutes));
    }
}
