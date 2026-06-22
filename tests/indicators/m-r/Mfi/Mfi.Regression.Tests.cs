namespace Regression;

[TestClass, TestCategory("Regression")]
public class MfiTests : RegressionTestBase<MfiResult>
{
    public MfiTests() : base("mfi.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToMfi(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToMfiList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Bars.ToMfiHub(14).Results.IsExactly(Expected);
}
