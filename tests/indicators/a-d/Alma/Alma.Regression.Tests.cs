namespace Regression;

[TestClass, TestCategory("Regression")]
public class AlmaTests : RegressionTestBase<AlmaResult>
{
    public AlmaTests() : base("alma.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToAlma(9, 0.85, 6).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToAlmaList(9, 0.85, 6).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToAlmaHub(9, 0.85, 6).Results.IsExactly(Expected);
}
