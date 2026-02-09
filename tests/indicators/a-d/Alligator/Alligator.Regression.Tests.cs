namespace Regression;

[TestClass, TestCategory("Regression")]
public class AlligatorTests : RegressionTestBase<AlligatorResult>
{
    public AlligatorTests() : base("alligator.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAlligator().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToAlligatorList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToAlligatorHub().Results.IsExactly(Expected);
}
