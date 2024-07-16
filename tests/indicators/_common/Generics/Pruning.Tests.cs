namespace Utilities;

[TestClass]
public class Pruning : TestBase
{
    [TestMethod]
    public void Remove()
    {
        // specific periods
        IEnumerable<HeikinAshiResult> results =
            Quotes.GetHeikinAshi()
              .RemoveWarmupPeriods(102);

        Assert.AreEqual(400, results.Count());

        // bad remove period
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetAdx().RemoveWarmupPeriods(-1));
    }

    [TestMethod]
    public void RemoveTooMany()
    {
        // more than available
        IEnumerable<HeikinAshiResult> results =
            Quotes.GetHeikinAshi()
              .RemoveWarmupPeriods(600);

        Assert.AreEqual(0, results.Count());
    }
}
