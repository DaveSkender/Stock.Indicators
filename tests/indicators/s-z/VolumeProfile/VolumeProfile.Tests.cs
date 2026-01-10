namespace Tests.Indicators;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class VpvrTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<VolumeProfileResult> results = quotes
        .GetVolumeProfile(0.01M)
        .ToList();

        // proper quantities
        Assert.AreEqual(quotes.Count(), results.Count);

        // dates align with source quotes
        int i = 0;
        foreach (Quote q in quotes)
        {
            Assert.AreEqual(q.Date, results[i].Date);
            i++;
        }

        // each result's volume profile should sum back to the original volume
        foreach (VolumeProfileResult r in results)
        {
            decimal profileSum = r.VolumeProfile.Sum(v => v.Volume);
            Assert.AreEqual(decimal.Round(r.Volume, 6), decimal.Round(profileSum, 6));
        }

        // cumulative profile on the last result should equal the total volume of all quotes
        decimal expectedTotal = quotes.Sum(q => q.Volume);
        VolumeProfileResult last = results.LastOrDefault();
        Assert.IsNotNull(last);
        decimal cumulativeSum = last.CumulativeVolumeProfile.Sum(v => v.Volume);
        Assert.AreEqual(decimal.Round(expectedTotal, 6), decimal.Round(cumulativeSum, 6));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<VolumeProfileResult> r = noquotes
        .GetVolumeProfile(0.01M)
        .ToList();

        Assert.AreEqual(0, r.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => quotes.GetVolumeProfile(0M).ToList());
    }

    [TestMethod]
    public void ZeroWidthRange()
    {
        List<Quote> single = new List<Quote>
        {
            new Quote { Date = DateTime.Parse("2020-01-01"), Open =100m, High =100m, Low =100m, Close =100m, Volume =1000m }
        };

        List<VolumeProfileResult> results = single.GetVolumeProfile(0.01M).ToList();
        Assert.AreEqual(1, results.Count);

        VolumeProfileResult r = results[0];
        Assert.AreEqual(1, r.VolumeProfile.Count());
        Assert.AreEqual(1000m, r.VolumeProfile.Sum(v => v.Volume));
        Assert.AreEqual(1000m, r.CumulativeVolumeProfile.Sum(v => v.Volume));
    }

    [TestMethod]
    public void CumulativeAcrossChain()
    {
        List<Quote> list = new List<Quote>
        {
            new Quote { Date = DateTime.Parse("2020-01-01"), Open =10m, High =12m, Low =10m, Close =11m, Volume =100m },
            new Quote { Date = DateTime.Parse("2020-01-02"), Open =11m, High =13m, Low =11m, Close =12m, Volume =200m }
        };

        List<VolumeProfileResult> results = list.GetVolumeProfile(1m).ToList();
        Assert.AreEqual(2, results.Count);

        Assert.AreEqual(100m, results[0].VolumeProfile.Sum(v => v.Volume));
        Assert.AreEqual(200m, results[1].VolumeProfile.Sum(v => v.Volume));

        decimal cum = results[1].CumulativeVolumeProfile.Sum(v => v.Volume);
        Assert.AreEqual(300m, cum);
    }

    [TestMethod]
    public void PrecisionBinsCount()
    {
        List<Quote> q = new List<Quote>
        {
            new Quote { Date = DateTime.Parse("2020-01-01"), Open =10m, High =12m, Low =10m, Close =11m, Volume =100m }
        };

        VolumeProfileResult r1 = q.GetVolumeProfile(1m).First();
        Assert.AreEqual(3, r1.VolumeProfile.Count()); //10,11,12

        VolumeProfileResult r2 = q.GetVolumeProfile(0.5m).First();
        Assert.AreEqual(5, r2.VolumeProfile.Count()); //10,10.5,11,11.5,12
    }
}
