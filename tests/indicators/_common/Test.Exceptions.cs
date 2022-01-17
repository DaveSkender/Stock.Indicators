using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class ExceptionTests : TestBase
{
    // bad quotes exceptions
    [TestMethod]
    [ExpectedException(typeof(BadQuotesException), "Bad quotes without message.")]
    public void BadQuotes()
    {
        throw new BadQuotesException();
    }

    [TestMethod]
    [ExpectedException(typeof(BadQuotesException), "Bad quotes with message.")]
    public void BadQuotesWithMessage()
    {
        throw new BadQuotesException("This is a quotes exception.");
    }

    [TestMethod]
    [ExpectedException(typeof(BadQuotesException), "Bad quotes with inner Exception.")]
    public void BadQuotesWithInner()
    {
        throw new BadQuotesException("This has an inner Exception.", new ArgumentException());
    }
}
