using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

// GLOBALS & INITIALIZATION OF TEST DATA

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Other")]
[assembly: InternalsVisibleTo("Tests.Performance")]
namespace Internal.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        internal static readonly CultureInfo englishCulture = new("en-US", false);

        internal static readonly IEnumerable<Quote> quotes = TestData.GetDefault();
        internal static readonly IEnumerable<Quote> otherQuotes = TestData.GetCompare();
        internal static readonly IEnumerable<Quote> badQuotes = TestData.GetBad();
        internal static readonly IEnumerable<Quote> longishQuotes = TestData.GetLongish();
        internal static readonly IEnumerable<Quote> longestQuotes = TestData.GetLongest();
    }
}
