namespace Regression;

[TestClass, TestCategory("Regression")]
public class TrixTests : RegressionTestBase<TrixResult>
{
    public TrixTests() : base("trix.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToTrix().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToTrixList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToTrixHub(14).Results.IsExactly(Expected);
}
