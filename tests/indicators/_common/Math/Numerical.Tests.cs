namespace Utilities;

[TestClass]
[TestCategory("Utilities")]
public class Numericals : TestBase
{
    private readonly double[] _closePrice = LongishQuotes
        .Select(static x => (double)x.Close)
        .ToArray();

    private readonly double[] _x = [1, 2, 3, 4, 5];
    private readonly double[] _y = [0, 0, 0, 0];

    [TestMethod]
    public void StdDev()
    {
        double sd = _closePrice.StdDev();

        Assert.AreEqual(633.932098287, Math.Round(sd, 9));
    }

    [TestMethod]
    public void StdDevNull()
        => FluentActions
            .Invoking(static () => Numerical.StdDev(null))
            .Should()
            .ThrowExactly<ArgumentNullException>();

    [TestMethod]
    public void Slope()
    {
        double s = Numerical.Slope(_x, _x);

        s.Should().Be(1d);
    }

    [TestMethod]
    public void SlopeXnull()
        => FluentActions
            .Invoking(() => Numerical.Slope(null, _x))
            .Should()
            .ThrowExactly<ArgumentNullException>();

    [TestMethod]
    public void SlopeYnull()
        => FluentActions
            .Invoking(() => Numerical.Slope(_x, null))
            .Should()
            .ThrowExactly<ArgumentNullException>();

    [TestMethod]
    public void SlopeMismatch()
        => FluentActions
            .Invoking(() => Numerical.Slope(_x, _y))
            .Should()
            .ThrowExactly<ArgumentException>();

    [TestMethod]
    public void RoundDownDate()
    {
        TimeSpan interval = PeriodSize.OneHour.ToTimeSpan();
        DateTime evDate = DateTime.Parse("2020-12-15 09:35:45", invariantCulture);

        DateTime rnDate = evDate.RoundDown(interval);
        DateTime exDate = DateTime.Parse("2020-12-15 09:00:00", invariantCulture);

        rnDate.Should().Be(exDate);
    }

    [TestMethod]
    public void ToTimeSpan()
    {
        Assert.AreEqual(PeriodSize.OneMinute.ToTimeSpan(), TimeSpan.FromMinutes(1));
        Assert.AreEqual(PeriodSize.TwoMinutes.ToTimeSpan(), TimeSpan.FromMinutes(2));
        Assert.AreEqual(PeriodSize.ThreeMinutes.ToTimeSpan(), TimeSpan.FromMinutes(3));
        Assert.AreEqual(PeriodSize.FiveMinutes.ToTimeSpan(), TimeSpan.FromMinutes(5));
        Assert.AreEqual(PeriodSize.FifteenMinutes.ToTimeSpan(), TimeSpan.FromHours(0.25));
        Assert.AreEqual(PeriodSize.ThirtyMinutes.ToTimeSpan(), TimeSpan.FromHours(0.5));
        Assert.AreEqual(PeriodSize.OneHour.ToTimeSpan(), TimeSpan.FromMinutes(60));
        Assert.AreEqual(PeriodSize.TwoHours.ToTimeSpan(), TimeSpan.FromHours(2));
        Assert.AreEqual(PeriodSize.FourHours.ToTimeSpan(), TimeSpan.FromHours(4));
        Assert.AreEqual(PeriodSize.Day.ToTimeSpan(), TimeSpan.FromHours(24));
        Assert.AreEqual(PeriodSize.Week.ToTimeSpan(), TimeSpan.FromDays(7));

        TimeSpan.Zero.Should().Be(PeriodSize.Month.ToTimeSpan());
    }
}
