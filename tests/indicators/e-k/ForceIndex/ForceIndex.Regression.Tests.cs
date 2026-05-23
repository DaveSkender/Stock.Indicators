namespace Regression;

[TestClass, TestCategory("Regression")]
public class ForceIndexTests : RegressionTestBase<ForceIndexResult>
{
    public ForceIndexTests() : base("force.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToForceIndex().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToForceIndexList(2).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToForceIndexHub().Results.IsExactly(Expected);
}
