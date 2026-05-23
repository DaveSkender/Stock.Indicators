namespace Regression;

[TestClass, TestCategory("Regression")]
public class MfiTests : RegressionTestBase<MfiResult>
{
    public MfiTests() : base("mfi.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToMfi(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToMfiList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToMfiHub(14).Results.IsExactly(Expected);
}
