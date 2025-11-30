namespace Tests.Indicators;

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class VpvrAnalysisTests : TestBase
{
 [TestMethod]
 public void Analysis()
 {
 // create series with typical precision used in other tests
 var results = quotes
 .GetVolumeProfile(0.01M)
 .ToList();

 // proper quantities
 Assert.AreEqual(502, results.Count);

 // at least some bins exist for the last result
 var last = results.Last();
 Assert.IsNotNull(last);
 Assert.IsTrue(last.VolumeProfile.Any());

 // cumulative totals should equal total volume
 decimal expectedTotal = quotes.Sum(q => q.Volume);
 decimal cumulativeSum = last.CumulativeVolumeProfile.Sum(v => v.Volume);
 Assert.AreEqual(expectedTotal, cumulativeSum);

 // the top cumulative bin should match the max volume in the cumulative profile
 var maxVolume = last.CumulativeVolumeProfile.Max(v => v.Volume);
 var top = last.CumulativeVolumeProfile.OrderByDescending(v => v.Volume).First();
 Assert.AreEqual(maxVolume, top.Volume);

 // basic usage sanity
 Assert.AreEqual(502, results.Count);

 // Emit values for regression snapshot
 Console.WriteLine($"TOTAL_VOLUME,{expectedTotal}");
 Console.WriteLine($"CUMULATIVE_BINS,{last.CumulativeVolumeProfile.Count()}");
 Console.WriteLine($"TOP_PRICE,{top.Price}");
 Console.WriteLine($"TOP_VOLUME,{top.Volume}");
 Console.WriteLine($"LAST_DATE,{last.Date:MM/dd/yyyy}");
 }
}
