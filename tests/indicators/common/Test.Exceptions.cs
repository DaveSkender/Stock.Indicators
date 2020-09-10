using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;

namespace Internal.Tests
{
    [TestClass]
    public class ExceptionTests : TestBase
    {

        // bad parameter exceptions
        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad parameter without message.")]
        public void BadParameter()
        {
            throw new BadParameterException();
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad parameter with message.")]
        public void BadParameterWithMessage()
        {
            throw new BadParameterException("This is a parameter exception.");
        }

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad parameter with inner Exception.")]
        public void BadParameterWithInner()
        {
            throw new BadParameterException("This has an inner Exception.", new Exception());
        }


        // bad history exceptions
        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Bad history without message.")]
        public void BadHistory()
        {
            throw new BadHistoryException();
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Bad history with message.")]
        public void BadHistoryWithMessage()
        {
            throw new BadHistoryException("This is a history exception.");
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Bad history with inner Exception.")]
        public void BadHistoryWithInner()
        {
            throw new BadHistoryException("This has an inner Exception.", new Exception());
        }

    }
}