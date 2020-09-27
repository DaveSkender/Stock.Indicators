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
    }
}
