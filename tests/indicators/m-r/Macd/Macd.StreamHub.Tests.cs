using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass]
public class MacdStreamHubTests : Test.Data.TestBase
{
    [TestMethod]
    public void StandardMacdHub()
    {
        // arrange
        Quote[] quotes = Quotes.Take(50).ToArray();
        QuoteHub<Quote> provider = new();
        
        // act - create hub and process quotes
        MacdHub<Quote> hub = provider.ToMacd(12, 26, 9);
        
        foreach (Quote quote in quotes)
        {
            provider.Add(quote);
        }

        // get results
        IReadOnlyList<MacdResult> hubResults = hub.Results;

        // assert - should match static implementation
        IReadOnlyList<MacdResult> expected = quotes.ToMacd(12, 26, 9);
        hubResults.Should().HaveCount(expected.Count);

        for (int i = 0; i < hubResults.Count; i++)
        {
            hubResults[i].Timestamp.Should().Be(expected[i].Timestamp);
            
            if (expected[i].Macd.HasValue)
            {
                hubResults[i].Macd.Should().BeApproximately(expected[i].Macd.Value, 1E-8);
            }
            else
            {
                hubResults[i].Macd.Should().BeNull();
            }

            if (expected[i].Signal.HasValue)
            {
                hubResults[i].Signal.Should().BeApproximately(expected[i].Signal.Value, 1E-8);
            }
            else
            {
                hubResults[i].Signal.Should().BeNull();
            }

            if (expected[i].Histogram.HasValue)
            {
                hubResults[i].Histogram.Should().BeApproximately(expected[i].Histogram.Value, 1E-8);
            }
            else
            {
                hubResults[i].Histogram.Should().BeNull();
            }

            if (expected[i].FastEma.HasValue)
            {
                hubResults[i].FastEma.Should().BeApproximately(expected[i].FastEma.Value, 1E-8);
            }
            else
            {
                hubResults[i].FastEma.Should().BeNull();
            }

            if (expected[i].SlowEma.HasValue)
            {
                hubResults[i].SlowEma.Should().BeApproximately(expected[i].SlowEma.Value, 1E-8);
            }
            else
            {
                hubResults[i].SlowEma.Should().BeNull();
            }
        }
    }
}