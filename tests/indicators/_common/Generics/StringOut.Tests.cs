using System.Globalization;
using Test.Utilities;

namespace Tests.Common;

[TestClass]
public class StringOutputs : TestBase
{
    [TestMethod]
    public void ToConsoleQuoteType()
    {
        DateTime timestamp = DateTime.TryParse(
            "2017-02-03", CultureInfo.InvariantCulture, out DateTime d) ? d : default;

        Quote quote = new(timestamp, 216.1579m, 216.875m, 215.84m, 216.67m, 98765432832);

        string sut = quote.ToConsole();
        string val = quote.ToStringOut();

        sut.Should().Be(val);
    }

    [TestMethod]
    public void ToStringOutQuoteType()
    {
        DateTime timestamp = DateTime.TryParse(
            "2017-02-03", CultureInfo.InvariantCulture, out DateTime d) ? d : default;

        Quote quote = new(timestamp, 216.1m, 216.875m, 215.84m, 216.67m, 85273832);

        string sut = StringOut.ToStringOut(quote);
        Console.WriteLine(sut);

        // note description has max of 30 "-" characters
        string expected = """
            Property   Type                    Value  Description
            ------------------------------------------------------------------------
            Timestamp  DateTime  2017-02-03T00:00:00  Close date/time of the aggregate
            Open       Decimal                 216.1  Aggregate bar's first tick price
            High       Decimal               216.875  Aggregate bar's highest tick price
            Low        Decimal                215.84  Aggregate bar's lowest tick price
            Close      Decimal                216.67  Aggregate bar's last tick price
            Volume     Decimal              85273832  Aggregate bar's tick volume
            """.WithDefaultLineEndings();

        sut.Should().Be(expected);
    }

    [TestMethod]
    public void ToStringOutMostTypes()
    {
        AllTypes allTypes = new();
        string sut = StringOut.ToStringOut(allTypes);
        Console.WriteLine(sut);

        string expected = """
            Property                Type                                          Value  Description
            -----------------------------------------------------------------------------------------------------------
            Timestamp               DateTime                       2023-01-01 14:30:00Z  A 'DateTime' type with time (UTC)
            DateTimeProperty        DateTime                        2023-01-01T09:30:00  A 'DateTime' type with time
            DateProperty            DateOnly                                 2023-01-01  A 'DateOnly' type without time.
            DateTimeOffsetProperty  DateTimeOffset    2023-01-01T09:30:00.0000000-05:00  A 'DateTimeOffset' type with time and offset.
            TimeSpanProperty        TimeSpan                                   01:02:03  A 'TimeSpan' type
            ByteProperty            Byte                                            255  A 'Byte' type
            ShortProperty           Int16                                         32767  A 'Int16' short integer type
            IntProperty             Int32                                   -2147483648  A 'Int32' integer type
            LongProperty            Int64                           9223372036854775803  'get' the 'Int64' long integer type
            FloatProperty           Single                                   -125.25143  A get of 'Single' floating point type
            DoubleProperty          Double                            5.251426433759354  A 'Double' floating point type
            DecimalProperty         Decimal              7922815.2514264337593543950335  A 'Decimal' type
            CharProperty            Char                                              A  A 'Char' type
            BoolProperty            Boolean                                        True  A 'Boolean' type
            NoXmlProperty           Boolean                                       False  
            StringProperty          String          The lazy dog jumped over the sly...  A 'String' type
            """.WithDefaultLineEndings();

        sut.Should().Be(expected);
    }

    [TestMethod]
    public void ToFixedWidthQuoteStandard()
    {
        string output = Quotes.Take(12).ToFixedWidth();
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
            """.WithDefaultLineEndings();

        output.Should().Be(expected);
    }

    [TestMethod]
    public void ToFixedWidthQuoteIntraday()
    {
        string output = Intraday.Take(12).ToFixedWidth();
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
            """.WithDefaultLineEndings();

        output.Should().Be(expected);
    }

    [TestMethod]
    public void ToFixedWidthMinutes()
    {
        List<Quote> quotes = [];
        for (int i = 0; i < 20; i++)
        {
            quotes.Add(new Quote(new DateTime(2023, 1, 1, 9, 30, 0).AddMinutes(i), 100 + i, 105 + i, 95 + i, 102 + i, 1000 + i));
        }

        string output = quotes.ToFixedWidth();
        Console.WriteLine(output);

        string[] lines = output.Split(Environment.NewLine);
        lines.Length.Should().Be(23); // 2 headers + 20 data rows

        Assert.Fail("test not implemented");
    }

    [TestMethod]
    public void ToFixedWidthSeconds()
    {
        List<Quote> quotes = [];
        for (int i = 0; i < 20; i++)
        {
            quotes.Add(new Quote(new DateTime(2023, 1, 1, 9, 30, 0).AddSeconds(i), 100 + i, 105 + i, 95 + i, 102 + i, 1000 + i));
        }

        string output = quotes.ToFixedWidth();
        Console.WriteLine(output);

        string[] lines = output.Split(Environment.NewLine);
        lines.Length.Should().Be(23); // 2 headers + 20 data rows

        Assert.Fail("test not implemented");
    }
}



/// <summary>
/// A test class implementing <see cref="ISeries"/> containing properties of various data types.
/// </summary>
public class AllTypes : ISeries
{
    /// <summary>
    /// A <see cref="DateTime"/> type with time (UTC)
    /// </summary>
    public DateTime Timestamp { get; } = new DateTime(2023, 1, 1, 14, 30, 0, DateTimeKind.Utc);

    /// <summary>
    /// A <see cref="DateTime"/> type with time
    /// </summary>
    public DateTime DateTimeProperty { get; } = new DateTime(2023, 1, 1, 9, 30, 0);

    /// <summary>
    /// A <see cref="DateOnly"/> type without time.
    /// </summary>
    public DateOnly DateProperty { get; } = new DateOnly(2023, 1, 1);

    /// <summary>
    /// A <see cref="DateTimeOffset"/> type with time and offset.
    /// </summary>
    public DateTimeOffset DateTimeOffsetProperty { get; } = new DateTimeOffset(2023, 1, 1, 9, 30, 0, TimeSpan.FromHours(-5));

    /// <summary>
    /// A <see cref="TimeSpan"/> type
    /// </summary>
    public TimeSpan TimeSpanProperty { get; } = new TimeSpan(1, 2, 3);

    /// <summary>
    /// A <see cref="byte"/> type
    /// </summary>
    public byte ByteProperty { get; } = 255;

    /// <summary>
    /// A <see cref="short"/> short integer type
    /// </summary>
    public short ShortProperty { get; } = 32767;

    /// <summary>
    /// A <see cref="int"/> integer type
    /// </summary>
    public int IntProperty { get; } = -2147483648;

    /// <summary>
    /// <see langword="get"/> the <see cref="long"/> long integer type
    /// </summary>
    public long LongProperty { get; } = 9223372036854775803L;

    /// <summary>
    /// A <code>get</code> of <see cref="float"/> floating point type
    /// </summary>
    public float FloatProperty { get; } = -125.25143f;

    /// <summary>
    /// A <see cref="double"/> floating point type
    /// </summary>
    public double DoubleProperty { get; } = 5.251426433759354d;

    /// <summary>
    /// A <see cref="decimal"/> type
    /// </summary>
    public decimal DecimalProperty { get; } = 7922815.2514264337593543950335m;

    /// <summary>
    /// A <see cref="char"/> type
    /// </summary>
    public char CharProperty { get; } = 'A';

    /// <summary>
    /// A <see cref="bool"/> type
    /// </summary>
    public bool BoolProperty { get; } = true;

    public bool NoXmlProperty { get; }  // false

    /// <summary>
    /// A <see cref="string"/> type
    /// </summary>
    public string StringProperty { get; } = "The lazy dog jumped over the sly brown fox.";
}
