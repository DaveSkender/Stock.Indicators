using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Data;
using Skender.Stock.Indicators;

namespace DebugTests;

[TestClass]
public class ConnorsRsiDebug : TestBase
{
    [TestMethod]
    public void ShowResults()
    {
        var results = Quotes.ToConnorsRsi(3, 2, 100).ToList();
        
        Console.WriteLine("Idx\tStreak\tRsi\tRsiStrk\tPctRnk\tCRSI");
        for (int i = 0; i < 10; i++)
        {
            var r = results[i];
            Console.WriteLine($"{i}\t{r.Streak:F1}\t{r.Rsi?.ToString("F2") ?? "null"}\t{r.RsiStreak?.ToString("F2") ?? "null"}\t{r.PercentRank?.ToString("F2") ?? "null"}\t{r.ConnorsRsi?.ToString("F2") ?? "null"}");
        }
    }
}
