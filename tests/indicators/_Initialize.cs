using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Globalization;

namespace Internal.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        internal static readonly CultureInfo englishCulture = new CultureInfo("en-US", false);

        internal static readonly IEnumerable<Quote> history = History.GetHistory();
        internal static readonly IEnumerable<Quote> historyOther = History.GetHistoryOther();
        internal static readonly IEnumerable<Quote> historyBad = History.GetHistoryBad();

        internal static readonly int[] convergeQuantities = 
            new int[] { 5, 20, 30, 50, 75, 100, 120, 150, 200, 250, 350, 500, 600, 700, 800, 900, 1000 };
    }
}
