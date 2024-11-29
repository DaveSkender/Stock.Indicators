namespace Tests.Common;

[TestClass]
public class ResultsToString : TestBase
{
    [TestMethod]
    public void ToStringFixedWidth()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("Timestamp"));
        Assert.IsTrue(output.Contains("Open"));
        Assert.IsTrue(output.Contains("High"));
        Assert.IsTrue(output.Contains("Low"));
        Assert.IsTrue(output.Contains("Close"));
        Assert.IsTrue(output.Contains("Volume"));
    }

    [TestMethod]
    public void ToStringCSV()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.CSV);
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("Timestamp,Open,High,Low,Close,Volume"));
    }

    [TestMethod]
    public void ToStringJson()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.JSON);
        Console.WriteLine(output);

        Assert.IsTrue(output.StartsWith("["));
        Assert.IsTrue(output.EndsWith("]"));
    }

    [TestMethod]
    public void ToStringWithLimitQty()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth, 4, 5);
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("Timestamp"));
        Assert.IsTrue(output.Contains("Open"));
        Assert.IsTrue(output.Contains("High"));
        Assert.IsTrue(output.Contains("Low"));
        Assert.IsTrue(output.Contains("Close"));
        Assert.IsTrue(output.Contains("Volume"));
    }

    [TestMethod]
    public void ToStringWithStartIndexAndEndIndex()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth, 4, 2, 5);
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("Timestamp"));
        Assert.IsTrue(output.Contains("Open"));
        Assert.IsTrue(output.Contains("High"));
        Assert.IsTrue(output.Contains("Low"));
        Assert.IsTrue(output.Contains("Close"));
        Assert.IsTrue(output.Contains("Volume"));
    }
}
