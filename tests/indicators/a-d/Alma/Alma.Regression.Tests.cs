namespace Regression;

[TestClass, TestCategory("Regression")]
public class AlmaTests : RegressionTestBase<AlmaResult>
{
    public AlmaTests() : base("alma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAlma(9, 0.85, 6).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => quoteHub.ToAlmaHub(9, 0.85, 6).Results.AssertEquals(Expected);
}
