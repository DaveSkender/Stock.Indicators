using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

[assembly: CLSCompliant(true)]
namespace Internal.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        internal static readonly CultureInfo englishCulture = new("en-US", false);

        internal static readonly IEnumerable<Quote> history = HistoryTestData.Get();
        internal static readonly IEnumerable<Quote> historyOther = HistoryTestData.GetCompare();
        internal static readonly IEnumerable<Quote> historyBad = HistoryTestData.GetBad();
    }
}
