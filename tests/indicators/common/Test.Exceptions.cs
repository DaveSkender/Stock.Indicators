using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class ExceptionTests : TestBase
    {

        // bad history exceptions
        [TestMethod]
        [ExpectedException(typeof(BadHistoryException), "Bad history without message.")]
        public void BadHistory()
        {
            throw new BadHistoryException();
        }

        [TestMethod]
        [ExpectedException(typeof(BadHistoryException), "Bad history with message.")]
        public void BadHistoryWithMessage()
        {
            throw new BadHistoryException("This is a history exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(BadHistoryException), "Bad history with inner Exception.")]
        public void BadHistoryWithInner()
        {
            throw new BadHistoryException("This has an inner Exception.", new ArgumentException());
        }
    }
}
