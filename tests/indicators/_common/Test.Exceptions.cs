using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class ExceptionTests : TestBase
{

    // bad quotes exceptions
    [TestMethod]
    [ExpectedException(typeof(BadQuotesException), "Bad quotes without message.")]
    public void BadHistory()
    {
        throw new BadQuotesException();
    }

    [TestMethod]
    [ExpectedException(typeof(BadQuotesException), "Bad quotes with message.")]
    public void BadHistoryWithMessage()
    {
        throw new BadQuotesException("This is a quotes exception.");
    }

    [TestMethod]
    [ExpectedException(typeof(BadQuotesException), "Bad quotes with inner Exception.")]
    public void BadHistoryWithInner()
    {
        throw new BadQuotesException("This has an inner Exception.", new ArgumentException());
    }
}
