namespace Regression;

[TestClass, TestCategory("Regression")]
public class RocWbTests : RegressionTestBase<RocWbResult>
{
    public RocWbTests() : base("roc-wb.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToRocWb().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToRocWbList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToRocWbHub().Results.IsExactly(Expected);
}
