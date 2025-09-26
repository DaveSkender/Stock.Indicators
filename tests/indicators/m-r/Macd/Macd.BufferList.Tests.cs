using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass]
public class MacdBufferListTests : Test.Data.TestBase
{
    [TestMethod]
    public void StandardMacdList()
    {
        // arrange
        MacdList macdList = new(12, 26, 9);
        Quote[] quotes = Quotes.Take(50).ToArray();

        // act
        foreach (Quote quote in quotes)
        {
            macdList.Add(quote);
        }

        // assert - should match static implementation  
        IReadOnlyList<MacdResult> expected = quotes.ToMacd(12, 26, 9);
        macdList.Should().HaveCount(expected.Count);

        for (int i = 0; i < macdList.Count; i++)
        {
            macdList[i].Timestamp.Should().Be(expected[i].Timestamp);
            
            if (expected[i].Macd.HasValue)
            {
                macdList[i].Macd.Should().BeApproximately(expected[i].Macd.Value, 1E-6);
            }
            else
            {
                macdList[i].Macd.Should().BeNull();
            }

            if (expected[i].Signal.HasValue)
            {
                macdList[i].Signal.Should().BeApproximately(expected[i].Signal.Value, 1E-6);
            }
            else
            {
                macdList[i].Signal.Should().BeNull();
            }

            if (expected[i].Histogram.HasValue)
            {
                macdList[i].Histogram.Should().BeApproximately(expected[i].Histogram.Value, 1E-6);
            }
            else
            {
                macdList[i].Histogram.Should().BeNull();
            }

            if (expected[i].FastEma.HasValue)
            {
                macdList[i].FastEma.Should().BeApproximately(expected[i].FastEma.Value, 1E-6);
            }
            else
            {
                macdList[i].FastEma.Should().BeNull();
            }

            if (expected[i].SlowEma.HasValue)
            {
                macdList[i].SlowEma.Should().BeApproximately(expected[i].SlowEma.Value, 1E-6);
            }
            else
            {
                macdList[i].SlowEma.Should().BeNull();
            }
        }
    }

    [TestMethod]
    public void MacdListProperties()
    {
        // arrange & act
        MacdList macdList = new(10, 20, 5);

        // assert
        macdList.FastPeriods.Should().Be(10);
        macdList.SlowPeriods.Should().Be(20);
        macdList.SignalPeriods.Should().Be(5);
    }
}