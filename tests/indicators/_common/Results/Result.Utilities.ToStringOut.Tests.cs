namespace Tests.Common;

[TestClass]
public class ResultsToString : TestBase
{
    [TestMethod]
    public void ToStringFixedWidth()
    {
        List<string> output = Quotes.ToMacd().Select(m => m.ToString()).ToList();
        Console.WriteLine(string.Join(Environment.NewLine, output));

        Assert.Fail("Test not implemented, very wrong syntax.");
    }

    [TestMethod]
    public void ToStringCSV()
    {
        // import quotes from CSV file
        List<string> output = Quotes.ToMacd().Select(m => m.ToString()).ToList();

        // recompose into CSV string
        string csvOutput = string.Join(",", output);

        // should be same as original
        Console.WriteLine(csvOutput);
        Assert.Fail("Test not implemented, very wrong syntax.");
    }

    [TestMethod]
    public void ToStringJson()
    {
        List<string> output = Quotes.ToMacd().Select(m => m.ToString()).ToList();
        string jsonOutput = System.Text.Json.JsonSerializer.Serialize(output);

        Console.WriteLine(jsonOutput);
        Assert.Fail("Test not implemented, very wrong syntax.");
    }
}
