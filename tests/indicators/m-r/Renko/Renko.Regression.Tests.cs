namespace Regression;

[TestClass, TestCategory("Regression")]
public class RenkoTests : RegressionTestBase<RenkoResult>
{
    public RenkoTests() : base("renko.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToRenko(1.0m).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToRenkoList(1.0m).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToRenkoHub(1.0m).Results.IsExactly(Expected);
}
