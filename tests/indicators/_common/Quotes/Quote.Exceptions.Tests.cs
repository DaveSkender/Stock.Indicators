namespace Utilities;

// invalid quotes exceptions

public partial class Quotes : TestBase
{
    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Bad quotes without message.")]
    public void InvalidQuotesExceptionThrow()
        => throw new InvalidQuotesException();

    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Bad quotes with message.")]
    public void InvalidQuotesExceptionThrowWithMessage()
        => throw new InvalidQuotesException("This is a quotes exception.");

    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Bad quotes with inner Exception.")]
    public void InvalidQuotesExceptionThrowWithInner()
        => throw new InvalidQuotesException("This has an inner Exception.", new ArgumentException());
}
