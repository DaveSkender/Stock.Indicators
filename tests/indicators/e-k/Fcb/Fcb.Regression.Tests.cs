namespace Regression;

[TestClass, TestCategory("Regression")]
public class FcbTests : RegressionTestBase<FcbResult>
{
    public FcbTests() : base("fcb.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToFcb(2).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToFcbList(2).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToFcbHub(2).Results.IsExactly(Expected);
}
