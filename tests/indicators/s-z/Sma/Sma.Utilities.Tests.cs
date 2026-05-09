namespace StaticSeries;

public partial class SmaTests : StaticSeriesTestBase
{
    [TestMethod]
    public void Average()
    {
        TimeValue[] results =
        [
            new TimeValue(Quotes[0].Timestamp, 0.0),
            new TimeValue(Quotes[1].Timestamp, 4.0),
            new TimeValue(Quotes[2].Timestamp, 8.0)
        ];

        // calculate
        double? mid = results.Average(2, 1);
        double? end = results.Average(2);

        // assert
        mid.Should().Be(2);
        end.Should().Be(6);
    }
}
