namespace Regression;

[TestClass, TestCategory("Regression")]
public class GatorTests : RegressionTestBase<GatorResult>
{
    public GatorTests() : base("gator.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToGator().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToGatorList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToGatorHub().Results.IsExactly(Expected);
}
