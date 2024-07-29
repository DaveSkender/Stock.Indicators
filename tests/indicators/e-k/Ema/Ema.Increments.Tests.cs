namespace Increments;

[TestClass]
public class EmaTests : IncrementsTestBase
{
    [TestMethod]
    public override void Standard()
    {
        var ema = new Ema(14);
    }

    [TestMethod]
    public override void ValueBased()
    {
        Assert.Fail("Test not implemented");
    }
}
