namespace Tests.Common;

[TestClass]
public class ResultsToString : TestBase
{
    [TestMethod]
    public void ToStringFixedWidth()
    {
        var output = quotes.GetMacd().ToStringOut();
        Console.WriteLine(output);
        Assert.Fail();
    }

    [TestMethod]
    public void ToStringCSV()
    {
        // import quotes from CSV file
        var output = quotes.GetMacd().ToStringOut(OutType.CSV);

        // recompose into CSV string

        // should be same as original
        Console.WriteLine(output);
        Assert.Fail();
    }

    [TestMethod]
    public void ToStringJson()
    {
        var output = quotes.GetMacd().ToStringOut(OutType.JSON);
        Console.WriteLine(output);
        Assert.Fail();
    }
}
