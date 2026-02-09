namespace Regression;

[TestClass, TestCategory("Regression")]
public class AlmaTests : RegressionTestBase<AlmaResult>
{
    public AlmaTests() : base("alma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAlma(9, 0.85, 6).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToAlmaList(9, 0.85, 6).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToAlmaHub(9, 0.85, 6).Results.IsExactly(Expected);
}
