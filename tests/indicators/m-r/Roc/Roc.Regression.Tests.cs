namespace Regression;

[TestClass, TestCategory("Regression")]
public class RocTests : RegressionTestBase<RocResult>
{
    public RocTests() : base("roc.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToRoc().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToRocList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToRocHub(14).Results.IsExactly(Expected);
}
