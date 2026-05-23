namespace Regression;

[TestClass, TestCategory("Regression")]
public class StdDevTests : RegressionTestBase<StdDevResult>
{
    public StdDevTests() : base("stdev.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToStdDev().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToStdDevList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToStdDevHub(14).Results.IsExactly(Expected);
}
