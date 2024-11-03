namespace StaticSeries;

public partial class Sma : StaticSeriesTestBase
{
    [TestMethod]
    public void Average()
    {
        QuotePart[] results = new[]
        {
            new QuotePart(Quotes[0].Timestamp, 0.0),
            new QuotePart(Quotes[1].Timestamp, 4.0),
            new QuotePart(Quotes[2].Timestamp, 8.0)
        };

        // sut
        double? mid = results.Average(2, 1);
        double? end = results.Average(2);

        // assert
        mid.Should().Be(2);
        end.Should().Be(6);
    }
}
