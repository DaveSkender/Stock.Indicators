namespace Regression;

[TestClass, TestCategory("Regression")]
public class ParabolicsarTests : RegressionTestBase<ParabolicSarResult>
{
    public ParabolicsarTests() : base("psar.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToParabolicSar(0.02, 0.2).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToParabolicSarList(0.02, 0.2).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToParabolicSarHub(0.02, 0.2).Results.IsExactly(Expected);
}
