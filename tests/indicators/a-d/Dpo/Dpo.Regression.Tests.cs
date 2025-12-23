namespace Regression;

[TestClass, TestCategory("Regression")]
public class DpoTests : RegressionTestBase<DpoResult>
{
    public DpoTests() : base("dpo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDpo().IsExactly(Expected);

    [TestMethod]
    public override void Buffer()
    {
        DpoList list = Quotes.ToDpoList(14);
        list.IsExactly(Expected.Take(list.Count));
    }

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not feasible - DPO requires lookahead");
}
