namespace Utilities;

// invalid quotes exceptions

public partial class Quotes : TestBase
{
    [TestMethod]
    public void InvalidQuotesExceptionThrow()
        => Assert.ThrowsException<InvalidQuotesException>(()
            => throw new InvalidQuotesException());

    [TestMethod]
    public void InvalidQuotesExceptionThrowWithMessage()
        => Assert.ThrowsException<InvalidQuotesException>(()
            => throw new InvalidQuotesException("This is a quotes exception."));

    [TestMethod]
    public void InvalidQuotesExceptionThrowWithInner()
        => Assert.ThrowsException<InvalidQuotesException>(()
            => throw new InvalidQuotesException("This has an inner Exception.", new ArgumentException()));
}
