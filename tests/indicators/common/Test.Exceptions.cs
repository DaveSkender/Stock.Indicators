using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class ExceptionTests : TestBase
    {

        // bad quotes exceptions
        [TestMethod]
        [ExpectedException(typeof(BadHistoryException), "Bad quotes without message.")]
        public void BadHistory()
        {
            throw new BadHistoryException();
        }

        [TestMethod]
        [ExpectedException(typeof(BadHistoryException), "Bad quotes with message.")]
        public void BadHistoryWithMessage()
        {
            throw new BadHistoryException("This is a quotes exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(BadHistoryException), "Bad quotes with inner Exception.")]
        public void BadHistoryWithInner()
        {
            throw new BadHistoryException("This has an inner Exception.", new ArgumentException());
        }
    }
}
