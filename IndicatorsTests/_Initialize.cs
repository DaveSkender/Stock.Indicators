using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System.Collections.Generic;

namespace StockIndicators.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        internal static IEnumerable<Quote> history;
        internal static IEnumerable<Quote> historyOther;

        [AssemblyInitialize]
        public static void Initialize()
        {
            history = History.GetHistory();
            historyOther = History.GetHistoryOther();
        }
    }
}
