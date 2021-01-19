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
        internal static readonly CultureInfo englishCulture = new CultureInfo("en-US", false);

        internal static readonly IEnumerable<Quote> history = HistoryTestData.Get();
        internal static readonly IEnumerable<Quote> historyOther = HistoryTestData.GetCompare();
        internal static readonly IEnumerable<Quote> historyBad = HistoryTestData.GetBad();

        internal static readonly int[] convergeQuantities =
            new int[] { 5, 20, 30, 50, 75, 100, 120, 150, 200, 250, 350, 500, 600, 700, 800, 900, 1000 };
    }
}
