using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass]
public class SmaBufferListTests : Test.Data.TestBase
{
    [TestMethod]
    public void StandardSmaList()
    {
        // arrange
        SmaList smaList = new(14);
        List<Quote> quotes = Quotes.Take(25).ToList();

        // act
        foreach (Quote quote in quotes)
        {
            smaList.Add(quote);
        }

        // assert - compare with static implementation
        IReadOnlyList<SmaResult> expected = quotes.ToSma(14);
        smaList.Should().HaveCount(25);
        smaList[12].Sma.Should().BeNull();
        smaList[13].Sma.Should().BeApproximately(expected[13].Sma!.Value, 1E-8);
        smaList[24].Sma.Should().BeApproximately(expected[24].Sma!.Value, 1E-8);
    }

    [TestMethod]
    public void SmaListFromReusableValues()
    {
        // arrange
        List<Quote> quotes = Quotes.Take(25).ToList();
        SmaList smaList = new(14);

        // act
        foreach (Quote quote in quotes)
        {
            smaList.Add(quote);
        }

        // assert - should match static implementation
        IReadOnlyList<SmaResult> expected = quotes.ToSma(14);
        smaList.Should().HaveCount(expected.Count);

        for (int i = 0; i < smaList.Count; i++)
        {
            smaList[i].Timestamp.Should().Be(expected[i].Timestamp);
            if (expected[i].Sma.HasValue)
            {
                smaList[i].Sma.Should().BeApproximately(expected[i].Sma.Value, 1E-8);
            }
            else
            {
                smaList[i].Sma.Should().BeNull();
            }
        }
    }
}