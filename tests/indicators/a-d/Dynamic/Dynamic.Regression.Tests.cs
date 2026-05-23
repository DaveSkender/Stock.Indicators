namespace Regression;

[TestClass, TestCategory("Regression")]
public class DynamicTests : RegressionTestBase<DynamicResult>
{
    public DynamicTests() : base("dynamic.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToDynamic(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToDynamicList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToDynamicHub(14, 0.6).Results.IsExactly(Expected);
}
