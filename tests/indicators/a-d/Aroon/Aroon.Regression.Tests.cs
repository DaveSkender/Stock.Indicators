namespace Regression;

[TestClass, TestCategory("Regression")]
public class AroonTests : RegressionTestBase<AroonResult>
{
    public AroonTests() : base("aroon.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToAroon(25).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToAroonList(25).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => BarHub.ToAroonHub(25).Results.IsExactly(Expected);
}
