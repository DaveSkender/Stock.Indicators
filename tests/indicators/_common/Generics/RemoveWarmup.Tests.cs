namespace Utilities;

[TestClass]
public class RemoveWarmup : TestBase
{
    [TestMethod]
    public void Standard()
    {
        // specific periods
        IReadOnlyList<HeikinAshiResult> results = Quotes
            .GetHeikinAshi()
            .RemoveWarmupPeriods(102);

        Assert.AreEqual(400, results.Count);

        // bad remove period
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetAdx().RemoveWarmupPeriods(-1));
    }

    [TestMethod]
    public void TooMany()
    {
        // more than available
        IReadOnlyList<HeikinAshiResult> results = Quotes
            .GetHeikinAshi()
            .RemoveWarmupPeriods(600);

        Assert.AreEqual(0, results.Count);
    }
}
