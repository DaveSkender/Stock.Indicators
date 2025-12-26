namespace Utilities;

// invalid quotes exceptions

public partial class Quotes : TestBase
{
    [TestMethod]
    public void BadHistory()
        => FluentActions
            .Invoking(static () => throw new InvalidQuotesException())
            .Should()
            .ThrowExactly<InvalidQuotesException>();

    [TestMethod]
    public void BadHistoryWithMessage()
        => FluentActions
            .Invoking(static () => throw new InvalidQuotesException("This is a quotes exception."))
            .Should()
            .ThrowExactly<InvalidQuotesException>();

    [TestMethod]
    public void BadHistoryWithInner()
        => Assert.ThrowsExactly<InvalidQuotesException>(
            static () => throw new InvalidQuotesException("This has an inner Exception.", new ArgumentException()));
}
