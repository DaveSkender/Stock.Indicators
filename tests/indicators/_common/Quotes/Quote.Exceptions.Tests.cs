namespace Utilities;

// invalid quotes exceptions

public partial class Quotes : TestBase
{
    [TestMethod]
    public void InvalidQuotesExceptionThrow()
        => Assert.ThrowsExactly<InvalidQuotesException>(()
            => throw new InvalidQuotesException());

    [TestMethod]
    public void InvalidQuotesExceptionThrowWithMessage()
        => Assert.ThrowsExactly<InvalidQuotesException>(()
            => throw new InvalidQuotesException("This is a quotes exception."));

    [TestMethod]
    public void InvalidQuotesExceptionThrowWithInner()
        => Assert.ThrowsExactly<InvalidQuotesException>(()
            => throw new InvalidQuotesException("This has an inner Exception.", new ArgumentException()));
}
