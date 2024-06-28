namespace Tests.Common;

[TestClass]
public class QuoteEqualityTests : TestBase
{
    internal readonly DateTime evalDate
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);

    [TestMethod]
    public void EqualQuotes()
    {
        Quote q1 = new() {
            Timestamp = evalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            Close = 1m,
            Volume = 100
        };

        Quote q2 = new() {
            Timestamp = evalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            Close = 1m,
            Volume = 100
        };

        Quote q3 = new() {
            Timestamp = evalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            Close = 2m,
            Volume = 99
        };

        Assert.IsTrue(Equals(q1, q2));
        Assert.IsFalse(Equals(q1, q3));

        Assert.IsTrue(q1.Equals(q2));
        Assert.IsFalse(q1.Equals(q3));

        Assert.IsTrue(q1 == q2);
        Assert.IsFalse(q1 == q3);

        Assert.IsFalse(q1 != q2);
        Assert.IsTrue(q1 != q3);
    }
}
