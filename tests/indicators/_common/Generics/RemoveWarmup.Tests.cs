namespace Utilities;

[TestClass]
public class RemoveWarmup : TestBase
{
    [TestMethod]
    public void Standard()
    {
        // specific periods
        IReadOnlyList<HeikinAshiResult> results = Quotes
            .ToHeikinAshi()
            .RemoveWarmupPeriods(102);

        Assert.HasCount(400, results);

        // bad remove period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToAdx().RemoveWarmupPeriods(-1));
    }

    [TestMethod]
    public void TooMany()
    {
        // more than available
        IReadOnlyList<HeikinAshiResult> results = Quotes
            .ToHeikinAshi()
            .RemoveWarmupPeriods(600);

        Assert.IsEmpty(results);
    }
}
