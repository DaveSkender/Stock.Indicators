namespace Regression;

[TestClass, TestCategory("Regression")]
public class ElderrayTests : RegressionTestBase<ElderRayResult>
{
    public ElderrayTests() : base("elder-ray.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToElderRay(13).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToElderRayList(13).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToElderRayHub(13).Results.IsExactly(Expected);
}
