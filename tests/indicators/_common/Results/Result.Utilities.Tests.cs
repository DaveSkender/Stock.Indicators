namespace Tests.Common;

[TestClass]
public class Results : SeriesTestBase
{
    [TestMethod]
    public void Condense()
    {
        List<AdxResult> results = Quotes
            .GetAdx()
            .ToList();

        // make a few more in the middle null and NaN
        results[249] = results[249] with { Adx = null };
        results[345] = results[345] with { Adx = double.NaN };

        List<AdxResult> r = results.Condense().ToList();

        // proper quantities
        Assert.AreEqual(473, r.Count);

        // sample values
        AdxResult last = r.LastOrDefault();
        Assert.AreEqual(17.7565, last.Pdi.Round(4));
        Assert.AreEqual(31.1510, last.Mdi.Round(4));
        Assert.AreEqual(34.2987, last.Adx.Round(4));
    }
}
