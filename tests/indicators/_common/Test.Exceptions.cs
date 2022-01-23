using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class ExceptionTests : TestBase
{
    // bad quotes exceptions
    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Bad quotes without message.")]
    public void BadHistory()
    {
        throw new InvalidQuotesException();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Bad quotes with message.")]
    public void BadHistoryWithMessage()
    {
        throw new InvalidQuotesException("This is a quotes exception.");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Bad quotes with inner Exception.")]
    public void BadHistoryWithInner()
    {
        throw new InvalidQuotesException("This has an inner Exception.", new ArgumentException());
    }
}
