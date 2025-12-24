namespace Utilities;

[TestClass]
public class RemoveWarmup : TestBase
{
    [TestMethod]
    public void Standard()
    {
        // specific periods
        IReadOnlyList<HeikinAshiResult> sut = Quotes
            .ToHeikinAshi()
            .RemoveWarmupPeriods(102);

        sut.Should().HaveCount(400);

        // bad remove period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToAdx().RemoveWarmupPeriods(-1));
    }

    [TestMethod]
    public void TooMany()
    {
        // more than available
        IReadOnlyList<HeikinAshiResult> sut = Quotes
            .ToHeikinAshi()
            .RemoveWarmupPeriods(600);

        sut.Should().BeEmpty();
    }
}
