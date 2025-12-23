namespace Regression;

[TestClass, TestCategory("Regression")]
public class RenkoatrTests : RegressionTestBase<RenkoResult>
{
    public RenkoatrTests() : base("renko-atr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRenkoAtr().IsExactly(Expected);

    public override void Buffer() => throw new NotImplementedException("Intentionally not implemented");
    public override void Stream() => throw new NotImplementedException("Intentionally not implemented");
}
