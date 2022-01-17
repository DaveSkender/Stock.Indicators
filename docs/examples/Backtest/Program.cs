using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Skender.Stock.Indicators;

namespace Backtest;

public class Program
{
    public static void Main()
    {
        // See ConsoleApp first.  This is more advanced.

        /* This is a basic 20-year backtest-style analysis of
         * Stochastic RSI.  It will buy-to-open (BTO) one share
         * when the Stoch RSI (%K) is below 20 and crosses over the 
         * Signal (%D). The reverse Sell-to-Close (STC) and 
         * Sell-To-Open (STO) occurs when the Stoch RSI is above 80 and
         * crosses below the Signal.
         * 
         * As a result, there will always be one open LONG or SHORT
         * position that is opened and closed at signal crossover
         * points in the overbought and oversold regions of the indicator.
         */

        // fetch historical quotes from data provider
        List<Quote> quotesList = GetHistoryFromFeed()
            .ToList();

        // calculate Stochastic RSI
        List<StochRsiResult> resultsList =
            quotesList
            .GetStochRsi(14, 14, 3, 1)
            .ToList();

        // initialize
        decimal trdPrice = 0;
        decimal trdQty = 0;
        decimal rlzGain = 0;

        Console.WriteLine("   Date         Close  StRSI Signal  Cross    Net Gains");
        Console.WriteLine("-------------------------------------------------------");

        // roll through history
        for (int i = 1; i < quotesList.Count; i++)
        {
            Quote q = quotesList[i];
            StochRsiResult e = resultsList[i];     // evaluation period
            StochRsiResult l = resultsList[i - 1]; // last (prior) period
            string cross = string.Empty;

            // unrealized gain on open trade
            decimal trdGain = trdQty * (q.Close - trdPrice);

            // check for LONG event
            // condition: Stoch RSI was <= 20 and Stoch RSI crosses over Signal
            if (l.StochRsi <= 20
             && l.StochRsi < l.Signal
             && e.StochRsi >= e.Signal
             && trdQty != 1)
            {
                // emulates BTC + BTO
                rlzGain += trdGain;
                trdQty = 1;
                trdPrice = q.Close;
                cross = "LONG";
            }

            // check for SHORT event
            // condition: Stoch RSI was >= 80 and Stoch RSI crosses under Signal
            if (l.StochRsi >= 80
             && l.StochRsi > l.Signal
             && e.StochRsi <= e.Signal
             && trdQty != -1)
            {
                // emulates STC + STO
                rlzGain += trdGain;
                trdQty = -1;
                trdPrice = q.Close;
                cross = "SHORT";
            }

            if (cross != string.Empty)
            {
                Console.WriteLine(
                $"{q.Date,10:yyyy-MM-dd} " +
                $"{q.Close,10:c2}" +
                $"{e.StochRsi,7:N1}" +
                $"{e.Signal,7:N1}" +
                $"{cross,7}" +
                $"{rlzGain + trdGain,13:c2}");
            }
        }
    }


    private static IEnumerable<Quote> GetHistoryFromFeed()
    {
        /************************************************************

         We're mocking a data provider here by simply importing a
         JSON file, a similar format of many public APIs.

         This approach will vary widely depending on where you are
         getting your quote history.

         See https://github.com/DaveSkender/Stock.Indicators/discussions/579
         for free or inexpensive market data providers and examples.

         The return type of IEnumerable<Quote> can also be List<Quote>
         or ICollection<Quote> or other IEnumerable compatible types.

         ************************************************************/

        string json = File.ReadAllText("quotes.data.json");

        List<Quote> quotes = JsonConvert.DeserializeObject<IReadOnlyCollection<Quote>>(json)
            .OrderBy(x => x.Date)
            .ToList();

        return quotes;
    }
}
