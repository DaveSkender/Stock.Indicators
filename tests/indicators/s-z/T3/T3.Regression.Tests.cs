using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class T3Tests : RegressionTestBase<T3Result>
{
    public T3Tests() : base("t3.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToT3(5, 0.7).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
