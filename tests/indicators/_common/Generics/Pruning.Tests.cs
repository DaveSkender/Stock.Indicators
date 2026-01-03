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

        Assert.HasCount(400, results);

        // bad remove period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetAdx(14).RemoveWarmupPeriods(-1));
    }

    [TestMethod]
    public void RemoveTooMany()
    {
        // more than available
        IEnumerable<HeikinAshiResult> results =
            quotes.GetHeikinAshi()
              .RemoveWarmupPeriods(600);

        Assert.IsEmpty(results);
    }
}
