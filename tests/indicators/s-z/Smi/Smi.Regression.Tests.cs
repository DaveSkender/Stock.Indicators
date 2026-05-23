namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmiTests : RegressionTestBase<SmiResult>
{
    public SmiTests() : base("smi.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToSmi().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToSmiList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToSmiHub().Results.IsExactly(Expected);
}
