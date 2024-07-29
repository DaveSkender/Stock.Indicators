namespace Utilities;

public partial class Quotes : TestBase
{
    [TestMethod]
    public void EqualQuotes()
    {
        Quote q1 = new(EvalDate, 1m, 1m, 1m, 1m, 100);
        Quote q2 = new(EvalDate, 1m, 1m, 1m, 1m, 100);
        Quote q3 = new(EvalDate, 1m, 1m, 1m, 2m, 99);

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
