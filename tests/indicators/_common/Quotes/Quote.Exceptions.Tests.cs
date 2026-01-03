namespace Tests.Common;

[TestClass]
public class CustomExceptions : TestBase
{
    // bad quotes exceptions
    [TestMethod]
    public void BadHistory()
        => Assert.ThrowsExactly<InvalidQuotesException>(static () => throw new InvalidQuotesException());

    [TestMethod]
    public void BadHistoryWithMessage()
        => Assert.ThrowsExactly<InvalidQuotesException>(static () => throw new InvalidQuotesException("This is a quotes exception."));

    [TestMethod]
    public void BadHistoryWithInner()
        => Assert.ThrowsExactly<InvalidQuotesException>(static () => throw new InvalidQuotesException("This has an inner Exception.", new ArgumentException()));
}
