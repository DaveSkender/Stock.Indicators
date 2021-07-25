using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    // TEST QUOTE IMPORTER
    internal static class Importer
    {
        private static readonly CultureInfo EnglishCulture = new("en-US", false);

        // importer / parser
        internal static Quote FromCsv(string csvLine)
        {
            if (string.IsNullOrEmpty(csvLine))
            {
                return new Quote();
            }

            string[] values = csvLine.Split(',');
            Quote quotes = new()
            {
                Date = Convert.ToDateTime(values[0], EnglishCulture),
                Open = Convert.ToDecimal(values[1], EnglishCulture),
                High = Convert.ToDecimal(values[2], EnglishCulture),
                Low = Convert.ToDecimal(values[3], EnglishCulture),
                Close = Convert.ToDecimal(values[4], EnglishCulture),
                Volume = Convert.ToDecimal(values[5], EnglishCulture)
            };
            return quotes;
        }
    }

    // IMPORT TEST DATA
    internal class TestData
    {

        // S&P 500 ~62 years of daily data
        internal static IEnumerable<Quote> GetSnP()
        {
            return File.ReadAllLines("data/snp-long.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v));
        }
    }
}
