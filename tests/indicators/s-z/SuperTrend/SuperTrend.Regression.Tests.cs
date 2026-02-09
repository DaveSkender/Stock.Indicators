namespace Regression;

[TestClass, TestCategory("Regression")]
public class SupertrendTests : RegressionTestBase<SuperTrendResult>
{
    public SupertrendTests() : base("supertrend.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSuperTrend(10, 3).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToSuperTrendList(10, 3).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToSuperTrendHub(10, 3).Results.IsExactly(Expected);
}
