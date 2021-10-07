using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Skender.Stock.Indicators;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main()
        {
            // fetch historical quotes from data provider
            IEnumerable<Quote> quotes = GetHistoryFromFeed();

            // calculate 10-period SMA
            IEnumerable<SmaResult> results = quotes.GetSma(10);

            // note: results are usually returned with the same
            // number of elements as the provided quotes;
            // see individual indicator docs for more information

            // show results
            Console.WriteLine("SMA Results ---------------------------");

            // --> only showing last 10 records for brevity
            foreach (SmaResult r in results.TakeLast(10))
            {
                Console.WriteLine($"SMA on {r.Date:u} was ${r.Sma:N3}");
            }

            // analyze results (compare to quote values)
            Console.WriteLine();
            Console.WriteLine("SMA Analysis --------------------------");

            List<Quote> quotesList = quotes
                .ToList();

            List<SmaResult> resultsList = results
                .ToList();

            // --> only showing ~25 records for brevity
            for (int i = quotesList.Count - 25; i < quotesList.Count; i++)
            {
                Quote q = quotesList[i];
                SmaResult r = resultsList[i];

                bool isBullish = (q.Close > r.Sma);

                Console.WriteLine($"SMA on {r.Date:u} was ${r.Sma:N3}"
                                + $" and Bullishness is {isBullish}");
            }
        }


        private static IEnumerable<Quote> GetHistoryFromFeed()
        {
            // were mocking a data provider here by simply
            // importing a JSON file, a similar format of many public APIs

            // see https://github.com/DaveSkender/Stock.Indicators/discussions/579
            // for recommended market data providers and examples

            // note: this approach will vary widely depending on where
            // you are getting your quote history

            string json = File.ReadAllText("quotes.data.json");

            List<Quote> quotes = JsonConvert.DeserializeObject<IReadOnlyCollection<Quote>>(json)
                .OrderBy(x => x.Date)
                .ToList();

            return quotes;
        }
    }
}
