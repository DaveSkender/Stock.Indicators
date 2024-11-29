namespace Tests.Common;

[TestClass]
public class ResultsToString : TestBase
{
    [TestMethod]
    public void ToStringFixedWidth()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Macd");
        output.Should().Contain("Histogram");
        output.Should().Contain("Signal");

        string[] lines = output.Split('\n');
        lines[0].Trim().Should().Be("Timestamp      Macd        Histogram   Signal     ");
        lines[1].Trim().Should().Be("2017-01-03     0.0000      0.0000      0.0000     ");
    }

    [TestMethod]
    public void ToStringCSV()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.CSV);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp,Macd,Histogram,Signal");

        string[] lines = output.Split('\n');
        lines[0].Trim().Should().Be("Timestamp,Macd,Histogram,Signal");
        lines[1].Trim().Should().Be("2017-01-03,0.0000,0.0000,0.0000");
    }

    [TestMethod]
    public void ToStringJson()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.JSON);
        Console.WriteLine(output);

        output.Should().StartWith("[");
        output.Should().EndWith("]");
    }

    [TestMethod]
    public void ToStringWithLimitQty()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth, 4);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Macd");
        output.Should().Contain("Histogram");
        output.Should().Contain("Signal");

        string[] lines = output.Split('\n');
        lines.Length.Should().Be(5); // 1 header + 4 data rows
    }

    [TestMethod]
    public void ToStringWithStartIndexAndEndIndex()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth, null, 2, 5);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Macd");
        output.Should().Contain("Histogram");
        output.Should().Contain("Signal");

        string[] lines = output.Split('\n');
        lines.Length.Should().Be(5); // 1 header + 4 data rows
    }

    [TestMethod]
    public void ToStringOutOrderDateFirst()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string[] lines = output.Split('\n');
        string headerLine = lines[0];
        string firstDataLine = lines[1];

        headerLine.Should().StartWith("Timestamp");
        firstDataLine.Should().StartWith("2017-01-03");
    }

    [TestMethod]
    public void ToStringOutProperUseOfOutType()
    {
        string outputFixedWidth = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        string outputCSV = Quotes.ToMacd().ToStringOut(OutType.CSV);
        string outputJSON = Quotes.ToMacd().ToStringOut(OutType.JSON);

        outputFixedWidth.Should().Contain("Timestamp");
        outputCSV.Should().Contain("Timestamp,Macd,Histogram,Signal");
        outputJSON.Should().StartWith("[");
        outputJSON.Should().EndWith("]");
    }

    [TestMethod]
    public void ToStringOutDateFormatting()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string[] lines = output.Split('\n');
        string firstDataLine = lines[1];

        firstDataLine.Should().StartWith("2017-01-03");
    }

    [TestMethod]
    public void ToStringOutPerformance()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        Console.WriteLine($"Elapsed time: {elapsedMs} ms");
        elapsedMs.Should().BeLessThan(500); // Ensure performance is within acceptable limits
    }

    [TestMethod]
    public void ToStringOutDifferentBaseListTypes()
    {
        string output = Quotes.ToCandle().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Open");
        output.Should().Contain("High");
        output.Should().Contain("Low");
        output.Should().Contain("Close");
        output.Should().Contain("Volume");
        output.Should().Contain("Size");
        output.Should().Contain("Body");
        output.Should().Contain("UpperWick");
        output.Should().Contain("LowerWick");

        string[] lines = output.Split('\n');
        lines[0].Trim().Should().Be("Timestamp      Open        High        Low         Close       Volume      Size        Body        UpperWick   LowerWick   ");
        lines[1].Trim().Should().Be("2017-01-03     212.71      213.35      211.52      212.57      96708880    1.83        0.14        0.64        0.18        ");
    }

    [TestMethod]
    public void ToStringOutWithMultipleIndicators()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Macd");
        output.Should().Contain("Histogram");
        output.Should().Contain("Signal");

        output = Quotes.ToAdx().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Pdi");
        output.Should().Contain("Mdi");
        output.Should().Contain("Adx");

        string[] lines = output.Split('\n');
        lines[0].Trim().Should().Be("Timestamp      Pdi         Mdi         Adx         ");
        lines[1].Trim().Should().Be("2017-01-03     0.0000      0.0000      0.0000      ");
    }

    [TestMethod]
    public void ToStringOutWithUniqueHeadersAndValues()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Macd");
        output.Should().Contain("Histogram");
        output.Should().Contain("Signal");

        output = Quotes.ToAdx().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Pdi");
        output.Should().Contain("Mdi");
        output.Should().Contain("Adx");

        string[] lines = output.Split('\n');
        lines[0].Trim().Should().Be("Timestamp      Pdi         Mdi         Adx         ");
        lines[1].Trim().Should().Be("2017-01-03     0.0000      0.0000      0.0000      ");
    }

    [TestMethod]
    public void ToStringOutWithListQuote()
    {
        string output = Quotes.ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Open");
        output.Should().Contain("High");
        output.Should().Contain("Low");
        output.Should().Contain("Close");
        output.Should().Contain("Volume");

        string[] lines = output.Split('\n');
        lines[0].Trim().Should().Be("Timestamp      Open        High        Low         Close       Volume      ");
        lines[1].Trim().Should().Be("2017-01-03     212.71      213.35      211.52      212.57      96708880    ");
    }

    [TestMethod]
    public void ToStringOutWithIntradayQuotes()
    {
        var intradayQuotes = new List<Quote>
        {
            new Quote(new DateTime(2023, 1, 1, 9, 30, 0), 100, 105, 95, 102, 1000),
            new Quote(new DateTime(2023, 1, 1, 9, 31, 0), 102, 106, 96, 103, 1100),
            new Quote(new DateTime(2023, 1, 1, 9, 32, 0), 103, 107, 97, 104, 1200),
            new Quote(new DateTime(2023, 1, 1, 9, 33, 0), 104, 108, 98, 105, 1300),
            new Quote(new DateTime(2023, 1, 1, 9, 34, 0), 105, 109, 99, 106, 1400)
        };

        string output = intradayQuotes.ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        output.Should().Contain("Timestamp");
        output.Should().Contain("Open");
        output.Should().Contain("High");
        output.Should().Contain("Low");
        output.Should().Contain("Close");
        output.Should().Contain("Volume");

        string[] lines = output.Split('\n');
        lines[0].Trim().Should().Be("Timestamp           Open        High        Low         Close       Volume      ");
        lines[1].Trim().Should().Be("2023-01-01 09:30    100.00      105.00      95.00       102.00      1000        ");
    }

    [TestMethod]
    public void ToStringOutWith20Rows()
    {
        var quotes = new List<Quote>();
        for (int i = 0; i < 20; i++)
        {
            quotes.Add(new Quote(new DateTime(2023, 1, 1, 9, 30, 0).AddMinutes(i), 100 + i, 105 + i, 95 + i, 102 + i, 1000 + i));
        }

        string output = quotes.ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string[] lines = output.Split('\n');
        lines.Length.Should().Be(21); // 1 header + 20 data rows
    }
}
