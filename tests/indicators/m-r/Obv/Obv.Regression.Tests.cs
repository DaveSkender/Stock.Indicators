namespace Regression;

[TestClass, TestCategory("Regression")]
public class ObvTests : RegressionTestBase<ObvResult>
{
    public ObvTests() : base("obv.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToObv().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToObvList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => BarHub.ToObvHub().Results.IsExactly(Expected);
}
