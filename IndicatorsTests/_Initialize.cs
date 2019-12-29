using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace StockIndicators.Tests
{
    [TestClass]
    public class TestBase
    {
        internal static IEnumerable<Quote> history;

        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            history = History.GetHistory();
        }
    }
}
