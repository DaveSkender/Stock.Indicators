namespace Regression;

[TestClass, TestCategory("Regression")]
public class PvoTests : RegressionTestBase<PvoResult>
{
    public PvoTests() : base("pvo.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToPvo(12, 26, 9).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToPvoList(12, 26, 9).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Bars.ToPvoHub(12, 26, 9).Results.IsExactly(Expected);
}
