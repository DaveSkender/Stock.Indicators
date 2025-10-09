using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class RocwbTests : RegressionTestBase<RocWbResult>
{
    public RocwbTests() : base("roc-wb.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRocWb(12, 3, 6).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
