namespace Utilities;

// invalid quotes exceptions

public partial class Quotes : TestBase
{
    [TestMethod]
    public void BadHistory()
        => Assert.ThrowsExactly<InvalidQuotesException>(
            () => throw new InvalidQuotesException());

    [TestMethod]
    public void BadHistoryWithMessage()
        => Assert.ThrowsExactly<InvalidQuotesException>(
            () => throw new InvalidQuotesException("This is a quotes exception."));

    [TestMethod]
    public void BadHistoryWithInner()
        => Assert.ThrowsExactly<InvalidQuotesException>(
            () => throw new InvalidQuotesException("This has an inner Exception.", new ArgumentException()));
}
