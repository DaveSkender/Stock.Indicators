namespace Regression;

[TestClass, TestCategory("Regression")]
public class AlligatorTests : RegressionTestBase<AlligatorResult>
{
    public AlligatorTests() : base("alligator.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToAlligator().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToAlligatorList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToAlligatorHub().Results.IsExactly(Expected);
}
