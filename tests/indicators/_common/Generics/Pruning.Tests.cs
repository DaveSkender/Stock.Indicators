using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Common;

[TestClass]
public class Pruning : TestBase
{
    [TestMethod]
    public void Remove()
    {
        // specific periods
        IEnumerable<HeikinAshiResult> results =
            quotes.GetHeikinAshi()
              .RemoveWarmupPeriods(102);

        Assert.AreEqual(400, results.Count());

        // bad remove period
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetAdx(14).RemoveWarmupPeriods(-1));
    }

    [TestMethod]
    public void RemoveTooMany()
    {
        // more than available
        IEnumerable<HeikinAshiResult> results =
            quotes.GetHeikinAshi()
              .RemoveWarmupPeriods(600);

        Assert.AreEqual(0, results.Count());
    }
}
