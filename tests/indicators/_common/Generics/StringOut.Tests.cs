using System.Diagnostics;

namespace Tests.Common;

[TestClass]
public class StringOut : TestBase
{
    [TestMethod]
    public void ToStringFixedWidth()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string header = "  i  Timestamp   Macd    Histogram  Signal  FastEma  SlowEma ";
        output.Should().Contain(header);

        string[] lines = output.Split(Environment.NewLine);
        lines[0].Should().Be(header);
        lines[1].Should().Be("  0  2017-01-03  000.00  000.00     000.00  0.0000   000.00  ");
    }

    [TestMethod]
    public void ToStringBigNumbers()
    {
        string output = Data.GetTooBig(50).ToMacd().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        output.Should().NotContain(",");
    }

    [TestMethod]
    public void ToStringCSV()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.CSV, numberPrecision: 2);
        //Console.WriteLine(output);

        string header = "Timestamp,Macd,Signal,Histogram,FastEma,SlowEma";
        output.Should().Contain(header);

        string[] lines = output.Split(Environment.NewLine);
        lines.Length.Should().Be(504);   // 1 header + 502 data rows + trailing newline
        lines[0].Should().Be(header);

        lines = lines.Skip(1).ToArray(); // remove header for index parity

        lines[0].Should().Be("2017-01-03,,,,,");
        lines[10].Should().Be("2017-01-18,,,,,");
        lines[11].Should().Be("2017-01-19,,,,213.98,");
        lines[25].Should().Be("2017-02-08,0.88,,,215.75,214.87");
        lines[33].Should().Be("2017-02-21,2.20,1.52,0.68,219.94,217.74");
        lines[501].Should().Be("2018-12-31,-6.22,-5.86,-0.36,245.50,251.72");
    }

    [TestMethod]
    public void ToStringCSVRandomQuotes()
    {
        List<Quote> quotes = Data.GetRandom(
            bars: 1000,
            periodSize: PeriodSize.Day,
            includeWeekends: false)
            .ToList();

        string output = quotes.ToStringOut(OutType.CSV, numberPrecision: 6);
        Console.WriteLine(output);

        Assert.Fail("test not implemented");
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

        string[] lines = output.Split(Environment.NewLine);
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

        string[] lines = output.Split(Environment.NewLine);
        lines.Length.Should().Be(5); // 1 header + 4 data rows
    }

    [TestMethod]
    public void ToStringOutOrderDateFirst()
    {
        string output = Quotes.ToMacd().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string[] lines = output.Split(Environment.NewLine);
        string headerLine = lines[0];
        string lineLine = lines[1];
        string firstDataLine = lines[2];

        headerLine.Should().Be("  i  Timestamp   Macd    Histogram  Signal  FastEma  SlowEma ");
        lineLine.Should().Be("-----------------------------------------------------------");
        firstDataLine.Should().StartWith("  0  2017-01-03");
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

        string[] lines = output.Split(Environment.NewLine);
        string firstDataLine = lines[2];

        firstDataLine.Should().StartWith("  0  2017-01-03");
    }

    [TestMethod]
    public void ToStringOutPerformance()
    {
        IReadOnlyList<MacdResult> results
            = LongestQuotes.ToMacd();

        Stopwatch watch = Stopwatch.StartNew();
        string output = results.ToStringOut(OutType.FixedWidth);
        watch.Stop();

        // in microseconds (µs)
        double elapsedµs = watch.ElapsedMilliseconds / 1000d;
        Console.WriteLine($"Elapsed time: {elapsedµs} µs");

        Console.WriteLine(output);

        // Performance should be fast
        elapsedµs.Should().BeLessThan(2);
    }

    [TestMethod]
    public void ToStringOutDifferentBaseListTypes()
    {
        string output = Quotes.ToCandles().ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string[] lines = output.Split(Environment.NewLine);
        lines[0].Should().Be("  i  Timestamp     Open     High    Low   Close    Volume  Size   Body  UpperWick  LowerWick");
        lines[1].Should().Be("  0  2017-01-03  212.71  213.35  211.52  212.57  96708880  1.83   0.14       0.64       0.18");
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

        string[] lines = output.Split(Environment.NewLine);
        lines[0].Should().Be("Timestamp      Pdi         Mdi         Adx         ");
        lines[1].Should().Be("2017-01-03     0.0000      0.0000      0.0000      ");
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

        string[] lines = output.Split(Environment.NewLine);
        lines[0].Should().Be("Timestamp      Pdi         Mdi         Adx         ");
        lines[1].Should().Be("2017-01-03     0.0000      0.0000      0.0000      ");
    }

    [TestMethod]
    public void ToStringOutWithListQuote()
    {
        string output = Quotes.Take(12).ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string expected = """
             i  Timestamp     Open    High     Low   Close       Volume
            -----------------------------------------------------------
             0  2017-01-03  212.61  213.35  211.52  212.80  96708880.00
             1  2017-01-04  213.16  214.22  213.15  214.06  83348752.00
             2  2017-01-05  213.77  214.06  213.02  213.89  82961968.00
             3  2017-01-06  214.02  215.17  213.42  214.66  75744152.00
             4  2017-01-09  214.38  214.53  213.91  213.95  49684316.00
             5  2017-01-10  213.97  214.89  213.52  213.95  67500792.00
             6  2017-01-11  213.86  214.55  213.13  214.55  79014928.00
             7  2017-01-12  213.99  214.22  212.53  214.02  76329760.00
             8  2017-01-13  214.21  214.84  214.17  214.51  66385084.00
             9  2017-01-17  213.81  214.25  213.33  213.75  64821664.00
            10  2017-01-18  214.02  214.27  213.42  214.22  57997156.00
            11  2017-01-19  214.31  214.46  212.96  213.43  70503512.00
            """;

        output.Should().Be(expected);
    }

    [TestMethod]
    public void ToStringOutWithIntradayQuotes()
    {
        string output = Intraday.Take(12).ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string expected = """
             i  Timestamp           Open    High     Low   Close     Volume
            ---------------------------------------------------------------
             0  2020-12-15 09:30  367.40  367.62  367.36  367.46  407870.00
             1  2020-12-15 09:31  367.48  367.48  367.19  367.19  173406.00
             2  2020-12-15 09:32  367.19  367.40  367.02  367.35  149240.00
             3  2020-12-15 09:33  367.35  367.64  367.35  367.59  197941.00
             4  2020-12-15 09:34  367.59  367.61  367.32  367.43  147919.00
             5  2020-12-15 09:35  367.43  367.65  367.26  367.34  170552.00
             6  2020-12-15 09:36  367.35  367.56  367.15  367.53  200528.00
             7  2020-12-15 09:37  367.54  367.72  367.34  367.47  117417.00
             8  2020-12-15 09:38  367.48  367.48  367.19  367.42  127936.00
             9  2020-12-15 09:39  367.44  367.60  367.30  367.57  150339.00
            10  2020-12-15 09:40  367.58  367.78  367.56  367.61  136414.00
            11  2020-12-15 09:41  367.61  367.64  367.45  367.60   98185.00
            """;

        output.Should().Be(expected);
    }

    [TestMethod]
    public void ToStringOutMinutes()
    {
        List<Quote> quotes = [];
        for (int i = 0; i < 20; i++)
        {
            quotes.Add(new Quote(new DateTime(2023, 1, 1, 9, 30, 0).AddMinutes(i), 100 + i, 105 + i, 95 + i, 102 + i, 1000 + i));
        }

        string output = quotes.ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string[] lines = output.Split(Environment.NewLine);
        lines.Length.Should().Be(23); // 2 headers + 20 data rows

        Assert.Fail("test not implemented");
    }

    [TestMethod]
    public void ToStringOutSeconds()
    {
        List<Quote> quotes = [];
        for (int i = 0; i < 20; i++)
        {
            quotes.Add(new Quote(new DateTime(2023, 1, 1, 9, 30, 0).AddSeconds(i), 100 + i, 105 + i, 95 + i, 102 + i, 1000 + i));
        }

        string output = quotes.ToStringOut(OutType.FixedWidth);
        Console.WriteLine(output);

        string[] lines = output.Split(Environment.NewLine);
        lines.Length.Should().Be(23); // 2 headers + 20 data rows

        Assert.Fail("test not implemented");
    }
}
