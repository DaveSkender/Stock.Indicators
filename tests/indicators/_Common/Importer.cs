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
            Quote quote = new();

            HandleOHLCV(quote, "D", values[0]);
            HandleOHLCV(quote, "O", values[1]);
            HandleOHLCV(quote, "H", values[2]);
            HandleOHLCV(quote, "L", values[3]);
            HandleOHLCV(quote, "C", values[4]);
            HandleOHLCV(quote, "V", values[5]);

            return quote;
        }

        private static void HandleOHLCV(Quote quote, string position, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            switch (position)
            {
                case "D":
                    quote.Date = Convert.ToDateTime(value, EnglishCulture);
                    break;
                case "O":
                    quote.Open = Convert.ToDecimal(value, EnglishCulture);
                    break;
                case "H":
                    quote.High = Convert.ToDecimal(value, EnglishCulture);
                    break;
                case "L":
                    quote.Low = Convert.ToDecimal(value, EnglishCulture);
                    break;
                case "C":
                    quote.Close = Convert.ToDecimal(value, EnglishCulture);
                    break;
                case "V":
                    quote.Volume = Convert.ToDecimal(value, EnglishCulture);
                    break;
                default:
                    break;
            }
        }
    }


    // IMPORT TEST DATA
    internal class TestData
    {

        // DEFAULT: S&P 500 ~2 years of daily data
        internal static IEnumerable<Quote> GetDefault(int days = 502)
        {
            return File.ReadAllLines("_Common/data/default.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .OrderByDescending(x => x.Date)
                .Take(days)
                .ToList();
        }

        // BAD DATA
        internal static IEnumerable<Quote> GetBad(int days = 502)
        {
            return File.ReadAllLines("_Common/data/bad.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .OrderByDescending(x => x.Date)
                .Take(days)
                .ToList();
        }

        // BITCOIN DATA
        internal static IEnumerable<Quote> GetBitcoin(int days = 1246)
        {
            return File.ReadAllLines("_Common/data/bitcoin.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .OrderByDescending(x => x.Date)
                .Take(days)
                .ToList();
        }

        // COMPARE DATA ~2 years of TSLA data (matches default time)
        internal static IEnumerable<Quote> GetCompare(int days = 502)
        {
            return File.ReadAllLines("_Common/data/compare.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .OrderByDescending(x => x.Date)
                .Take(days)
                .ToList();
        }

        // INTRADAY DATA
        internal static IEnumerable<Quote> GetIntraday(int days = 1564)
        {
            return File.ReadAllLines("_Common/data/intraday.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .OrderByDescending(x => x.Date)
                .Take(days)
                .ToList();
        }

        // LONGISH DATA ~20 years of S&P 500 daily data
        internal static IEnumerable<Quote> GetLongish(int days = 5285)
        {
            return File.ReadAllLines("_Common/data/longish.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .OrderByDescending(x => x.Date)
                .Take(days)
                .ToList();
        }

        // LONGEST DATA ~62 years of S&P 500 daily data
        internal static IEnumerable<Quote> GetLongest()
        {
            return File.ReadAllLines("_Common/data/longest.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .ToList();
        }

        // PENNY DATA
        internal static IEnumerable<Quote> GetPenny()
        {
            return File.ReadAllLines("_Common/data/penny.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .ToList();
        }

        // MISMATCH DATA is in incorrect sequence
        internal static IEnumerable<Quote> GetMismatch()
        {
            return File.ReadAllLines("_Common/data/mismatch.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .ToList();
        }

        // ZIGZAG DATA
        internal static IEnumerable<Quote> GetZigZag(int days = 342)
        {
            return File.ReadAllLines("_Common/data/intraday.csv")
                .Skip(1)
                .Select(v => Importer.FromCsv(v))
                .OrderByDescending(x => x.Date)
                .Take(days)
                .ToList();
        }

    }
}
