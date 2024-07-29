namespace Utilities;

public partial class Quotes : TestBase
{
    // invalid quotes exceptions
    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Bad quotes without message.")]
    public void ThrowInvalidQuotesException()
        => throw new InvalidQuotesException();

    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Bad quotes with message.")]
    public void ThrowInvalidQuotesExWithMessage()
        => throw new InvalidQuotesException("This is a quotes exception.");

    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Bad quotes with inner Exception.")]
    public void ThrowInvalidQuotesExWithInner()
        => throw new InvalidQuotesException("This has an inner Exception.", new ArgumentException());
}
